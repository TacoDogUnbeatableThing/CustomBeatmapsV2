using UnityEngine;

namespace CustomBeatmaps
{
    public static class GUILookHelper
    {
        private static Texture2D _background;

        static GUILookHelper()
        {
            _background = new Texture2D(1,1);
            _background.SetPixel(0, 0, Color.black);
        }
        public static void SetGUISkin()
        {
            if (CustomBeatmaps.Instance.Settings.DarkMode)
            {
                GUI.backgroundColor = Color.black;
            }

            /*
            //GUI.skin.window.normal.background = _background;
            GUI.skin.window.onNormal = GUI.skin.window.normal;
            GUI.skin.window.onActive = GUI.skin.window.normal;
            GUI.skin.window.onFocused = GUI.skin.window.normal;
            GUI.skin.window.onHover = GUI.skin.window.normal;
            GUI.skin.window.focused = GUI.skin.window.normal;
            GUI.skin.window.hover = GUI.skin.window.normal;
            */
        }
    }
}
