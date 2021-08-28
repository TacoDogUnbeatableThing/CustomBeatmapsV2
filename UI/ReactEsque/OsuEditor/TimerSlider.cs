using System;
using CustomBeatmaps.UISystem;
using UnityEngine;

namespace CustomBeatmaps.UI.ReactEsque.OsuEditor
{
    public static class TimerSlider
    {
        private static bool Diff(float a, float b)
        {
            return Math.Abs(a - b) > 0.1f;
        }
        public static void Render(float time, float totalTime, Action<float> onSetTime,
            params GUILayoutOption[] options)
        {
            GUILayout.HorizontalSlider(time, 0, totalTime, options);

            GUI.Label(new Rect(0, 0, 128, 128), $"TIME: {time:000.00}s, {(int) (100 * time / totalTime)}%");
        }
    }
}
