using System;
using System.Collections.Generic;
using System.IO;
using CustomBeatmaps.UISystem;
using UnityEngine;

namespace CustomBeatmaps.UI.ReactEsque
{
    public static class OSUPackagePicker
    {
        public static void Render(string[] osuMaps, Action<string> onOpen, string errorMessage)
        {
            (Vector2 scrollPos, var setScrollPos) = Reacc.UseState(Vector2.zero);
            (string selected, var setSelected) = Reacc.UseState("");

            GUILayout.Label("Pick an OSU map to open it in EDIT MODE");
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            if (string.IsNullOrEmpty(errorMessage))
            {
                Vector2 newScroll = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true));
                setScrollPos.Invoke(newScroll);
                foreach (string osuMap in osuMaps)
                {
                    string name = Path.GetFileName(osuMap);
                    if (GUILayout.Button(name))
                    {
                        setSelected.Invoke(osuMap);
                    }
                }

                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.TextField(errorMessage);
            }

            GUILayout.EndVertical();
            if (!string.IsNullOrEmpty(selected))
            {
                if (GUILayout.Button($"EDIT: {Path.GetFileName(selected)}"))
                {
                    onOpen.Invoke(selected);
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}
