using System;
using System.Collections.Generic;
using BepInEx;
using CustomBeatmaps.Packages;
using CustomBeatmaps.Patches;
using CustomBeatmaps.UI.ReactEsque;
using CustomBeatmaps.UI.Structure;
using HarmonyLib;
using UnityEngine;

namespace CustomBeatmaps
{
    [BepInPlugin("tacodog.unbeatable.custombeatmaps", "Unbeatable Custom Beatmaps Plugin v2", "2.0.0")]
    public class CustomBeatmaps : BaseUnityPlugin
    {
        public static CustomBeatmaps Instance;

        private static readonly string BEATMAP_RELPATH = "USER_PACKAGES";

        private static readonly string SETTINGS_RELPATH = "CustomBeatmapSettings.json";

        private static readonly string LOCAL_TEMP_MP3_RELPATH = ".osu_edit_temp.mp3";

        private PackageGrabber _packageGrabber;

        private OldModConverter _oldModConverter;

        private ICustomBeatmapUIMain _uiMain;

        private TemporaryCopiedFile _localTempMp3File;

        private Settings _settings;

        private string UserPackageDirectory =>
            $"{UnbeatableDirectory}/{BEATMAP_RELPATH}";

        public static string UnbeatableDirectory =>
            Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/')).Replace('\\', '/');

        private void Awake()
        {
            Instance = this;
            Debug.Log("ADDED IN");
            SetCustomGUISkin();

            _packageGrabber = new PackageGrabber(UserPackageDirectory);
            _oldModConverter = new OldModConverter(UserPackageDirectory, "USER_BEATMAPS", ".conversions");
            _localTempMp3File = new TemporaryCopiedFile(LOCAL_TEMP_MP3_RELPATH);
            _localTempMp3File.Cleanup();

            _settings = Settings.Load(SETTINGS_RELPATH);
            FileWatchHelper.WatchFileForModifications(SETTINGS_RELPATH, path =>
            {
                Console.WriteLine("RELOADING SETTINGS");
                _settings = Settings.Load(path);
            });

            Harmony.CreateAndPatchAll(typeof(BeatmapParserLoadOverridePatch));
            Harmony.CreateAndPatchAll(typeof(BeatmapInfoAudioKeyOverridePatch));
            Harmony.CreateAndPatchAll(typeof(MainMenuLoadPatch));
            Harmony.CreateAndPatchAll(typeof(MemorySkipAfterCustomBeatmapPatch));

            MainMenuLoadPatch.OnOpen += () =>
            {
                _uiMain = new CustomBeatmapUIRenderer();
                _uiMain.Init(new CustomBeatmapUIMainProps(
                    _packageGrabber,
                    OnPlayRequest,
                    OnDownloadRequest,
                    DoOnlineSearch,
                    DoLocalSearch,
                    DoLeaderboardSearch,
                    GetOnlinePackageCount,
                    GetLocalPackageCount,
                    _oldModConverter.DetectOldBeatmaps(),
                    DoConvertOldBeatmaps,
                    DoOsuLocalSearch,
                    OnEditOsuMap
                ));
                _uiMain.Open();
                // Some extra cleanup never hurt nobody
                _localTempMp3File.Cleanup();
                // We're no longer loading a custom beatmap if we're in the main menu.
                BeatmapParserLoadOverridePatch.ResetOverrideBeatmap();
            };
        }

        private void DoConvertOldBeatmaps(bool moveDontCopy, Action<string> onLog)
        {
            _oldModConverter.ConvertFiles(moveDontCopy, onLog);
        }

        private void OnEditOsuMap(string beatmapPath)
        {
            // Make a copy of our Mp3 locally and set that local path as the audio key.
            var bmap = PackageHelper.LoadBeatmap(beatmapPath);
            string fullAudioPath = bmap.RealAudioKey;
            string localMp3 = $"../../{_localTempMp3File.CopyNewFile(fullAudioPath)}";
            Debug.Log($"OPENING (full Audio: {fullAudioPath} -> {localMp3}");
            bmap.RealAudioKey = localMp3;
            UnbeatableHelper.PlayBeatmap(bmap, true);
        }

        private void DoOsuLocalSearch(Action<string[]> onSearch, Action<string> onFail)
        {
            string[] osuBeatmaps;
            if (OsuHelper.GetOsuBeatmaps(_settings.OsuSongPathOverride, out osuBeatmaps))
            {
                onSearch.Invoke(osuBeatmaps);
            }
            else
            {
                onFail.Invoke($"Failed to find beatmaps at {OsuHelper.GetOsuPath(_settings.OsuSongPathOverride)}." +
                              $" You may override this setting by editing {SETTINGS_RELPATH}.");
            }
        }

        private void GetLocalPackageCount(Action<int> getter)
        {
            getter.Invoke(_packageGrabber.GetLocalPackageCount());
        }

        private void GetOnlinePackageCount(Action<int> getter)
        {
            // TODO: Query database for total package count.
            getter.Invoke(0);
        }

        private void DoLeaderboardSearch(UniqueId id, string difficulty, Action<LeaderboardInfo> getter)
        {
            // TODO: Search, later.
            getter.Invoke(LeaderboardInfo.Empty);
        }

        private void OnDownloadRequest(UniqueId id)
        {
            // TODO: Begin download locally from online, given id.
            // TODO: BeatmapPlayer: Cancel previous "play", but keep the download going.
        }

        private void DoLocalSearch(SearchQuery searchQuery, Action<IList<CustomPackageInfo>> getter)
        {
            Debug.Log($"DOING LOCAL SEARCH: [{searchQuery.StartPackage}, {searchQuery.EndPackage}");
            getter.Invoke(_packageGrabber.GetLocalPackagesSearched(searchQuery));
        }

        private void DoOnlineSearch(SearchQuery searchQuery, Action<IList<CustomPackageInfo>> getter)
        {
            // TODO: Jane's PR, potentially.
        }

        private void OnPlayRequest(UniqueId id, string difficulty)
        {
            // TODO: If beatmap is not downloaded, download first then play.
            UnbeatableHelper.PlayBeatmap(_packageGrabber.GetLocalBeatmap(id, difficulty));
            _uiMain?.Close();
        }

        public void ShowError(Exception e)
        {
            Debug.LogError(e);
            // TODO: Display an in-game popup or something
        }

        private void SetCustomGUISkin()
        {
            var skin = ScriptableObject.CreateInstance<GUISkin>();
            skin.window.normal.background = Texture2D.grayTexture;
        }
    }
}
