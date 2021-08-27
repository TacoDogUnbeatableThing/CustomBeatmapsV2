using System;
using CustomBeatmaps.UISystem;
using UnityEngine;

namespace CustomBeatmaps.UI.ReactEsque.OsuEditor
{
    public static class BottomPanel
    {
        public static void Render(float pad, float height, Action onRender)
        {
            float w = Screen.width,
                h = Screen.height;

            GUILayout.Window(Reacc.GetUniqueId(), new Rect(pad, h - height - pad, w - 2 * pad, height), _ =>
            {
                GUILayout.BeginHorizontal();
                onRender.Invoke();
                GUILayout.EndHorizontal();
            }, "");
        }
    }
}
