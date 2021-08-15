using System;
using System.Collections.Generic;
using BepInEx;
using CustomBeatmaps.Packages;
using CustomBeatmaps.UI.ReactEsque;
using CustomBeatmaps.UI.Structure;
using UnityEngine;

namespace CustomBeatmaps
{
    [BepInPlugin("tacodog.unbeatable.custombeatmaps", "Unbeatable Custom Beatmaps Plugin v2", "2.0.0")]
    public class CustomBeatmaps : BaseUnityPlugin
    {
        public static CustomBeatmaps Instance;

        private static readonly string BEATMAP_RELPATH = "USER_PACKAGES";

        private PackageGrabber _packageGrabber;

        private ICustomBeatmapUIMain _uiMain;

        private string UserPackageDirectory =>
            $"{Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'))}/{BEATMAP_RELPATH}";

        private void Awake()
        {
            Instance = this;
            Debug.Log("ADDED IN");
            SetCustomGUISkin();

            _packageGrabber = new PackageGrabber(UserPackageDirectory);

            _uiMain = new CustomBeatmapUIRenderer();
            _uiMain.Init(new CustomBeatmapUIMainProps(
                _packageGrabber,
                OnPlayRequest,
                OnDownloadRequest,
                DoOnlineSearch,
                DoLocalSearch,
                DoLeaderboardSearch,
                GetOnlinePackageCount,
                GetLocalPackageCount 
                ));
        }

        private void GetLocalPackageCount(Action<int> getter)
        {
            getter.Invoke(_packageGrabber.GetLocalPackages().Count);
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
            // TODO: Grab local files as an ICollection, sort them based on query and get 'em
            throw new NotImplementedException();
        }

        private void DoOnlineSearch(SearchQuery searchQuery, Action<IList<CustomPackageInfo>> getter)
        {
            // TODO: Jane's PR, potentially.
            throw new NotImplementedException();
        }

        private void OnPlayRequest(UniqueId id, string difficulty)
        {
            // TODO: make a BeatmapPlayer object, and tell it that we want to play this beatmap.
            //      If it's not downloaded, start a download and play afterwards!
            throw new NotImplementedException();
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