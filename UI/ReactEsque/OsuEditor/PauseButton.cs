using System;
using UnityEngine;

namespace CustomBeatmaps.UI.ReactEsque.OsuEditor
{
    public static class PauseButton
    {
        public static void Render(bool paused, Action<bool> onSetPaused, params GUILayoutOption[] options)
        {
            if (GUILayout.Button(
                paused ? "UNPAUSE" : "PAUSE", options))
            {
                onSetPaused(!paused);
            }
        }
    }
}
