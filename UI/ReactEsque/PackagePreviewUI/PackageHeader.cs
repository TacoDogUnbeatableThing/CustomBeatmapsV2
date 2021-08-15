using CustomBeatmaps.Packages;
using UnityEngine;

namespace CustomBeatmaps.UI.ReactEsque.PackagePreviewUI
{
    public static class PackageHeader
    {
        public static void Render(CustomPackageInfo info)
        {
            // TODO: In the future, an Icon maybe?
            GUILayout.Label($"Name: {info.PackageName}");
            GUILayout.Label($"Date: {info.Date}");
        }
    }
}
