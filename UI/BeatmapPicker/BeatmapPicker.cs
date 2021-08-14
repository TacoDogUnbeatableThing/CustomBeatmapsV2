using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CustomBeatmaps.Packages;
using UnityEngine;

namespace CustomBeatmaps.UI.BeatmapPicker
{
    public static class BeatmapPicker
    {
        public static void Render(Action<PackageId, string> onLocalBeatmapPlay, ICollection<CustomPackageInfo> localPackages)
        {
            (bool online, Action<bool> setOnline) = UI.UseState(false);
            (PackageId selectedPackage, Action<PackageId> setSelectedPackageId) = UI.UseState(PackageId.Empty);

            GUILayout.Window(UI.GetUniqueId(), new Rect(16, 16, Screen.width - 32, Screen.height - 32), _ =>
            {
                RenderOnlineModeSelector(online, setOnline);

                GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                        GUILayout.BeginHorizontal();
                            GUILayout.TextArea("Search goes here");
                            GUILayout.Button("Sort: Newest", GUILayout.ExpandWidth(false));
                        GUILayout.EndHorizontal();
                        List<CustomPackageInfo> list = new List<CustomPackageInfo>();
                        foreach (var package in localPackages) list.Add(package);
                        PackageList.Render(
                            32,
                            localPackages.Count, 
                            (start, end) => GetStrippedPackagesFromLocal(localPackages, start, end),
                            setSelectedPackageId
                        );
                        //GUILayout.Label("Rendered Song List goes Here", GUILayout.ExpandHeight(true));
                    GUILayout.EndVertical();
                    PackagePreview.Renderer(selectedPackage, difficulty => onLocalBeatmapPlay(selectedPackage, difficulty));
                    GUILayout.Label("Render Song with scoreboard and stuff");
                GUILayout.EndHorizontal();
                GUILayout.Label($"Online? {online}");

            }, "Beatmap Picker");
        }

        private static ICollection<CustomPackageStrippedInfo> GetStrippedPackagesFromLocal(
            ICollection<CustomPackageInfo> localPackages, int start, int end)
        {
            return localPackages.Select((package) => new CustomPackageStrippedInfo(package))
                .Where((_, index) => start <= index && index < end).ToArray();
        }

        private static void RenderOnlineModeSelector(bool online, Action<bool> onOnlineSet)
        {
            bool newOnline = GUILayout.Toolbar(online ? 1 : 0, new[] {"Local", "Online"}) == 1;
            if (newOnline != online)
            {
                onOnlineSet(newOnline);
            }
        }
    }
}
