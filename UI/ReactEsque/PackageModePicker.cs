using System;
using UnityEngine;

namespace CustomBeatmaps.UI.ReactEsque
{
    public static class PackageModePicker
    {
        public static void Render<T>(T mode, Action<T> setMode) where T : Enum
        {
            T newOnline = (T) typeof(T).GetEnumValues().GetValue(GUILayout.Toolbar((int)(object)mode, typeof(T).GetEnumNames()));
            if (!newOnline.Equals(mode))
            {
                setMode(newOnline);
            }
        }
    }
}
