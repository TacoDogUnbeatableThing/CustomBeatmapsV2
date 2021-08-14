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
            _uiMain.Init(_packageGrabber, OnPlay, OnDownloadRequest, DoOnlineSearch, DoLocalSearch);
        }

        private void OnDownloadRequest(UniqueId obj)
        {
            throw new NotImplementedException();
        }

        private void DoLocalSearch(SearchQuery searchQuery, Action<ICollection<CustomPackageLocalData>> onLoad)
        {
            // TODO: Grab local files as an ICollection, sort them based on query and get 'em
            throw new NotImplementedException();
        }

        private void DoOnlineSearch(SearchQuery searchQuery, Action<ICollection<CustomPackageInfo>> onLoad)
        {
            throw new NotImplementedException();
        }

        private void OnPlay(CustomBeatmapInfo obj)
        {
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