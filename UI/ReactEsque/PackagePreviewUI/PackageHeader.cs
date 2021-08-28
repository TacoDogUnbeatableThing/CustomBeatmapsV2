using CustomBeatmaps.Packages;
using UnityEngine;

namespace CustomBeatmaps.UI.ReactEsque.PackagePreviewUI
{
    public static class PackageHeader
    {
        public static void Render(CustomPackageInfo info)
        {
            // TODO: In the future, an Icon maybe?
            GUILayout.Label($"<size=40>{info.name}</size>");
            GUILayout.Label($"Date: {info.date}");
        }
    }
}
