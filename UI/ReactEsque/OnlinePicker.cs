using System;
using UnityEngine;

namespace CustomBeatmaps.UI.ReactEsque
{
    public static class OnlinePicker
    {
        public static void Render(bool online, Action<bool> setOnline)
        {
            bool newOnline = GUILayout.Toolbar(online ? 1 : 0, new[] {"Local", "Online"}) == 1;
            if (newOnline != online)
            {
                setOnline(newOnline);
            }
        }
    }
}