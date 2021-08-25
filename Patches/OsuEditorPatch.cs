using System.IO;
using HarmonyLib;
using UnityEngine;

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
        private static bool _reloadFlag;

        private static bool Enabled => _editMode && !string.IsNullOrEmpty(_editPath);
        public static void SetEditMode(bool editMode, string path=null)
        {
            _editMode = editMode;
            _editPath = path;
        }

        [HarmonyPatch(typeof(Rhythm.RhythmController), "Start")]
        [HarmonyPostfix]
        private static void StartPost(Rhythm.RhythmController __instance)
        {
            if (Enabled)
            {
                // Enable editing mode
                _watcher = FileWatchHelper.WatchFile(_editPath, OnFileReload);
                // TODO: Create edit UI
            }
        }

        // TODO: OnUpdate, do edit stuff!! If the reload flag is set, do the reloading there.

        private static void OnFileReload(string path)
        {
            if (path != _editPath || !_editMode)
            {
                Debug.Log($"Old file update detected ({path} != {_editPath}), cancelling edit mode");
                if (_watcher != null)
                {
                    _watcher.Dispose();
                    _watcher = null;
                }
                _editMode = false;
                _editPath = "";
                return;
            }

            _reloadFlag = true;
        }
    }
}
