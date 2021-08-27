using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using CustomBeatmaps.Packages;
using CustomBeatmaps.UI.ReactEsque.OsuEditor;
using CustomBeatmaps.UI.Structure;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using HarmonyLib;
using Rhythm;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Debug = UnityEngine.Debug;
using Object = System.Object;

namespace CustomBeatmaps.Patches
{
    /// <summary>
    /// If specified, an open beatmap will have extra UI added in to control beatmap gameplay.
    /// Also invincibility mode will be on.
    /// </summary>
    public static class OsuEditorPatch
    {
        private static bool _editMode;
        private static string _editPath;

        private static FileSystemWatcher _watcher;

        private enum ReloadFlag
        {
            None, Disk, Time
        }
        private static ReloadFlag _reloadFlag = ReloadFlag.None;

        private static OsuEditorUIRenderer _ui;

        // Cached info
        private static float _songDurationEstimate = -1;
        private static Rhythm.RhythmController _rhythmControllerInstance;
        private static EventInstance _musicInstance;

        private static bool Enabled => _editMode && !string.IsNullOrEmpty(_editPath);
        public static void SetEditMode(bool editMode, string path=null)
        {
            _editMode = editMode;
            _editPath = path.Replace('\\', '/');
        }

        [HarmonyPatch(typeof(Rhythm.RhythmController), "Start")]
        [HarmonyPostfix]
        private static void StartPost(Rhythm.RhythmController __instance, EventInstance ___musicInstance)
        {
            if (Enabled)
            {
                Debug.Log("OPENED OSU EDIT VIEW");
                _rhythmControllerInstance = __instance;
                _musicInstance = ___musicInstance;

                __instance.noFailOverride = true;

                // Enable editing mode
                _watcher = FileWatchHelper.WatchFile(_editPath, OnFileReload);

                // Show UI
                _ui = new OsuEditorUIRenderer();
                _ui.Init(new OsuUIMainProps(OnSetPaused, OnSetTime, GetPaused, GetCurrentTime, GetSongTotalLength));

                // Some other caching/initialization
                _songDurationEstimate = GetSongDurationEstimate(__instance.beatmap, ___musicInstance);
            }
        }

        [HarmonyPatch(typeof(Rhythm.RhythmController), "Update")]
        [HarmonyPostfix]
        private static void UpdatePost(Rhythm.RhythmController __instance, EventInstance ___musicInstance, ref GameObject ___noteGroup, ref Queue<FlipInfo> ___flips, ref Queue<NoteInfo> ___notes, ref Queue<CommandInfo> ___commands)
        {
            switch (_reloadFlag)
            {
                // TODO: Cache notes somehow so we don't reload from disk whenever we reset time.
                case ReloadFlag.Disk:
                case ReloadFlag.Time:
                    Debug.Log("RELOAD: FROM DISK");
                    ReloadSongFromDisk(__instance, ref ___noteGroup, ref ___flips, ref ___notes, ref ___commands);
                    _songDurationEstimate = GetSongDurationEstimate(__instance.beatmap, ___musicInstance);
                    ReloadSongAtCurrentPointVisual(__instance, ref ___noteGroup, ref ___flips, ref ___notes, ref ___commands);
                    /*
                    break;
                    Debug.Log("RELOAD: JUST SET TIME");
                    _songDurationEstimate = GetSongDurationEstimate(__instance.beatmap, ___musicInstance);
                    ReloadSongAtCurrentPointVisual(__instance, ref ___noteGroup, ref ___flips, ref ___notes, ref ___commands);
                    break;
                    */
                    break;
            }
            _reloadFlag = ReloadFlag.None;
        }

        // Exposed methods
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Rhythm.RhythmController), "UpdateFlips")]
        public static void UpdateFlips(object instance)
        {
            // It's a stub
        }
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Rhythm.RhythmController), "FixedUpdate")]
        public static void FixedUpdate(object instance)
        {
            // It's a stub
        }

        private static float GetSongTotalLength()
        {
            return (_songDurationEstimate > 0) ? _songDurationEstimate : 1;
        }

        private static float GetCurrentTime()
        {
            int position;
            _musicInstance.getTimelinePosition(out position);
            return (float) position / 1000f;
        }

        private static bool GetPaused()
        {
            bool paused;
            _musicInstance.getPaused(out paused);
            return paused;
        }

        private static void OnSetTime(float time)
        {
            return;
            Debug.Log($"TIME => {time}");
            var result = _musicInstance.setTimelinePosition((int) (time * 1000));
            // TODO: This sucks, I can't figure out how to get this to work with programmer sounds.
            // The janky but surefire way is to go back to importing ManagedBass... But that'll make this more
            // bloatier :( Might be worth it though.
            _rhythmControllerInstance.songSource.time = time;

            if (result != RESULT.OK)
            {
                Debug.Log($"FAILED: {result}");
            }
            _reloadFlag = ReloadFlag.Time;
        }

        private static void OnSetPaused(bool paused)
        {
            Debug.Log($"PAUSED: {paused}");
            _musicInstance.setPaused(paused);
            _rhythmControllerInstance.play = !paused;
            JeffBezosController.SetTimeScale(paused? 0 : 1, 0);
        }

        private static float GetSongDurationEstimate(Beatmap beatmap, EventInstance musicInstance)
        {
            if (beatmap.notes.Count != 0)
            {
                return (beatmap.notes.Last().time / 1000f) + 1;
            }
            // TODO: Estimate time from musicInstance (it gets it in ms, remember), and take the larger time value?
            return 1f;
        }

        private static void ReloadSongFromDisk(Rhythm.RhythmController instance, ref GameObject noteGroup,
            ref Queue<FlipInfo> flips, ref Queue<NoteInfo> notes, ref Queue<CommandInfo> commands)
        {
            var parser = instance.parser;
            var info = PackageGrabberUtils.LoadBeatmap(_editPath);
            parser.beatmap = ScriptableObject.CreateInstance<Beatmap>();
            string text = File.ReadAllText(_editPath);
            BeatmapParserEngine beatmapParserEngine = new BeatmapParserEngine();
            beatmapParserEngine.ReadBeatmap(text, ref parser.beatmap);
            parser.audioKey = info.RealAudioKey;
            instance.beatmap = parser.beatmap;

            flips = new Queue<FlipInfo>(instance.beatmap.flips);
            notes = new Queue<NoteInfo>(instance.beatmap.notes);
            commands = new Queue<CommandInfo>(instance.beatmap.commands);
        }

        private static void ReloadSongAtCurrentPointVisual(Rhythm.RhythmController instance, ref GameObject noteGroup,
            ref Queue<FlipInfo> flips, ref Queue<NoteInfo> notes, ref Queue<CommandInfo> commands)
        {
            // Hacky fix to fix song position getting offset?
            float songPos = GetCurrentTime() * 1000f + instance.playerTimingOffsetMs;
            instance.song.songPosition = songPos;

            // Force to current point in time
            while (notes.Count > 0 && (instance.song.songPosition >= notes.Peek().time ||
                                       instance.freestyleParent != null))
            {
                notes.Dequeue();
            }
            while (commands.Count > 0 && instance.song.songPosition >= (float) commands.Peek().start)
            {
                commands.Dequeue();
            }
            RecalculateFlipsAndCamCenter(instance, ref flips);

            // Kill all notes on screen
            foreach (BaseNote note in noteGroup.GetComponentsInChildren<BaseNote>())
            {
                // Kinda dangerous? Test to make sure this doesn't break something.
                GameObject.DestroyImmediate(note.gameObject);
            }

            // Force a fixed update to reload
            FixedUpdate(instance);

            instance.score = new Score(5f, instance.leniencyMilliseconds, instance.beatmap.GetSignificantNoteCount(), 20, 4);

        }

        private static void RecalculateFlipsAndCamCenter(Rhythm.RhythmController instance, ref Queue<FlipInfo> flips)
        {
            // Default to right side
            instance.player.ChangeSide(Side.Right);
            instance.cameraObject.SetTargetPoint(instance.rightCameraTargetPoint);
            instance.cameraIsCentered = false;
            // Parse all flips until we arrive at our current position.
            while (flips.Count > 0 && instance.song.songPosition >= (float) flips.Peek().time)
            {
                UpdateFlips(instance);
            }
        }


        private static void OnFileReload(string path)
        {
            path = path.Replace('\\', '/');
            if (path != _editPath || !_editMode)
            {
                Console.WriteLine($"Old file update detected ({path} != {_editPath}), cancelling edit mode");
                if (_watcher != null)
                {
                    _watcher.Dispose();
                    _watcher = null;
                }
                _editMode = false;
                _editPath = "";
                return;
            }

            _reloadFlag = ReloadFlag.Disk;
        }
    }
}
