using System.Diagnostics;
using UnityEngine;

namespace CustomBeatmaps.UI.ReactEsque.PackagePreviewUI
{
    public static class AnimatedDownloadText
    {
        public static void Render()
        {
            // Animates with dots, for fun
            float seconds = (float) Stopwatch.GetTimestamp() / (float)Stopwatch.Frequency;
            int cycle = (int) (seconds * 3) % 3 + 1;
            string dots = new string('.', cycle);
            GUILayout.Label($"downloading{dots}");
        }
    }
}