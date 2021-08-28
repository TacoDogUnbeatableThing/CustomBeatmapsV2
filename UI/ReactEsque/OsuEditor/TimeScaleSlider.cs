using System;
using CustomBeatmaps.UISystem;
using UnityEngine;

namespace CustomBeatmaps.UI.ReactEsque.OsuEditor
{
    public static class TimeScaleSlider
    {

        public static void Render(Action<float> onTimeScaleSet, params GUILayoutOption[] options)
        {
            (float timeScale, var setTimeScale) = Reacc.UseState(1f);
            (float timeScaleMax, var setTimeScaleMax) = Reacc.UseState(50f);

            // TODO: Negative speeds?
            timeScale = GUILayout.HorizontalSlider(timeScale, 0.5f, timeScaleMax, options);
            GUILayout.Label($"{timeScale:0.0}x",GUILayout.ExpandWidth(false));
            setTimeScale.Invoke(timeScale);
            onTimeScaleSet.Invoke(timeScale);

            // Some easing out to zero
            if (!Input.GetMouseButton(0))
            {
                setTimeScale(timeScale + (1 - timeScale) * 0.1f);
            }
        }
    }
}
