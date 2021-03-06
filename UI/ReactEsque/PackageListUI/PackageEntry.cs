using System;
using CustomBeatmaps.Packages;
using UnityEngine;

namespace CustomBeatmaps.UI.ReactEsque.PackageListUI
{
    public static class PackageEntry
    {
        public static void Render(CustomPackageInfo info, bool selected, Action onClick)
        {
            string label = $"{info.name}";
            if (selected)
            {
                label = $"<b>{label}</b>";
            }

            if (GUILayout.Button(label, GUILayout.ExpandWidth(true)))
            {
                onClick.Invoke();
            }
        }
    }
}
