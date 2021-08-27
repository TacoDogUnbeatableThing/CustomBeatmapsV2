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
            (float visualPos, var setVisualPos) = Reacc.UseState(0f);

            float newPos = GUILayout.HorizontalSlider(visualPos, 0, totalTime, options);

            GUI.Label(new Rect(0, 0, 128, 128), $"TIME: {time}ms");

            // Difference must be > 1 second to count.
            bool different = Diff(newPos, time);
            if (different)
            {
                visualPos = newPos;
                setVisualPos(visualPos);
            }
            else
            {
                visualPos = time;
                setVisualPos(time);
            }

            // Only set time once our slider has been released.
            // We're being conservative here, since reloading the entire file every frame is slow.
            if (Input.GetMouseButtonUp(0) && different)
            {
                onSetTime(visualPos);
                // Snap back in case if we fail to reset
                setVisualPos(time);
            } else if (!Input.GetMouseButton(0))
            {
                // No mouse, just set automatically.
                setVisualPos(time);
            }
        }
    }
}
