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

        private PackageGrabber _packageGrabber;

        private OldModConverter _oldModConverter;

        private ICustomBeatmapUIMain _uiMain;

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

            Harmony.CreateAndPatchAll(typeof(BeatmapParserLoadOverridePatch));
            Harmony.CreateAndPatchAll(typeof(BeatmapInfoAudioKeyOverridePatch));
            Harmony.CreateAndPatchAll(typeof(MainMenuLoadPatch));

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
                    (moveDontCopy, onLog) =>
                    {
                        _oldModConverter.ConvertFiles(moveDontCopy, onLog);
                    }
                ));
                _uiMain.Open();
            };
        }

        private void GetLocalPackageCount(Action<int> getter)
        {
            getter.Invoke(_packageGrabber.GetLocalPackageCount());
        }

        private void GetOnlinePackageCount(Action<int> getter)
        {
            // TODO: Query database for total package count.
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        private void DoLocalSearch(SearchQuery searchQuery, Action<IList<CustomPackageLocalData>> getter)
        {
            Debug.Log($"DOING LOCAL SEARCH: [{searchQuery.StartPackage}, {searchQuery.EndPackage}");
            getter.Invoke(_packageGrabber.GetLocalPackagesSearched(searchQuery));
        }

        private void DoOnlineSearch(SearchQuery searchQuery, Action<IList<CustomPackageInfo>> getter)
        {
            // TODO: Jane's PR, potentially.
            throw new NotImplementedException();
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
