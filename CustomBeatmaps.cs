using System;
using BepInEx;
using CustomBeatmaps.Packages;
using CustomBeatmaps.UI.BeatmapPicker;
using UnityEngine;

namespace CustomBeatmaps
{
    [BepInPlugin("tacodog.unbeatable.custombeatmaps", "Unbeatable Custom Beatmaps Plugin v2", "2.0.0")]
    public class CustomBeatmaps : BaseUnityPlugin
    {
        public static CustomBeatmaps Instance;

        private static readonly string BEATMAP_RELPATH = "USER_PACKAGES";

        private BeatmapPickerRenderer _beatmapPickerRenderer;

        public string UserPackageDirectory =>
            $"{Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'))}/{BEATMAP_RELPATH}";

        public PackageGrabber PackageGrabber;

        private void Awake()
        {
            Instance = this;
            SetCustomGUISkin();

            PackageGrabber = new PackageGrabber(UserPackageDirectory);

            Debug.Log("ADDED IN");
            _beatmapPickerRenderer = new BeatmapPickerRenderer(beatmap =>
            {
                Debug.Log($"TEST: {beatmap}");
            });

            // TODO: Delete, move to patch.
            DontDestroyOnLoad(_beatmapPickerRenderer.GameObject);
        }

        public void ShowError(Exception e)
        {
            Debug.LogError(e);
            // TODO: Display an in-game popup or something
        }

        private void SetCustomGUISkin()
        {
            GUISkin skin = ScriptableObject.CreateInstance<GUISkin>();
            skin.window.normal.background = Texture2D.grayTexture;
        }
    }
}
