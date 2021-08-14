
using System;
using System.Collections.Generic;
using System.IO;
using CustomBeatmaps.Packages;
using UnityEngine;

namespace CustomBeatmaps.UI.BeatmapPicker
{
    public class BeatmapPickerRenderer : UIRenderer
    {

        private readonly Action<CustomBeatmapInfo> _onLocalBeatmapPlay;

        // You can imagine this as kinda being like initializing React's renderer.
        // Except we pass some extra functions/controls that will be called down the line.
        // We do this instead of using a PURE functional context, because we may want to replace
        // this with a NON functional system in the future. Like if we want it to be fancy
        // and flashy, we won't be using functional programming, but this interface will still work.
        public BeatmapPickerRenderer(Action<CustomBeatmapInfo> onLocalBeatmapPlay)
            : base("BeatmapPicker")
        {
            _onLocalBeatmapPlay = onLocalBeatmapPlay;
        }

        // Some Public Controls
        public void Open()
        {
            GameObject.SetActive(true);
        }
        public void Close()
        {
            GameObject.SetActive(false);
        }

        // THIS is where rendering happens.
        // I'm definitely in my "functional rendering is cool and hip" phase
        protected override void OnUnityGUI()
        {
            BeatmapPicker.Render(OnLocalBeatmapPlay, CustomBeatmaps.Instance.PackageGrabber.GetLocalCustomPackages());
        }

        private void OnLocalBeatmapPlay(PackageId packageId, string difficulty)
        {
            CustomPackageInfo package = CustomBeatmaps.Instance.PackageGrabber.GetLocalCustomPackage(packageId);
            foreach (CustomBeatmapInfo beatmap in package.Beatmaps)
            {
                if (beatmap.difficulty == difficulty)
                {
                    _onLocalBeatmapPlay(beatmap);
                    return;
                }
            }

            CustomBeatmaps.Instance.ShowError(new InvalidOperationException($"No difficulty found for package {packageId}"));
        }
    }
}
