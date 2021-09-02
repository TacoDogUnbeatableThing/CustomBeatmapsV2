using System;
using System.Collections.Generic;
using System.IO;
using CustomBeatmaps.UISystem;
using UnityEngine;

namespace CustomBeatmaps.UI.ReactEsque
{
    public static class OSUPackagePicker
    {
        public static void Render(string[] osuMaps, Action<string> onOpen, Action<string, Action<string>> onOsuExport, string errorMessage)
        {
            (Vector2 scrollPos, var setScrollPos) = Reacc.UseState(Vector2.zero);
            (string selected, var setSelected) = Reacc.UseState("");
            (string exportMessage, var setExportMessage) = Reacc.UseState("Press EXPORT to export locally.");

            // Reset export message if we pick a different map.
            Reacc.UseEffect(() =>
            {
                setExportMessage.Invoke("Press EXPORT to export locally.");
            }, new object[]{selected});

            GUILayout.BeginVertical();
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
            GUILayout.BeginVertical();
                GUILayout.Label(exportMessage);
                GUILayout.FlexibleSpace();
                if (!string.IsNullOrEmpty(selected))
                {
                    string pathName = Path.GetFileName(selected);
                    if (GUILayout.Button($"EXPORT: {pathName}"))
                    {
                        onOsuExport.Invoke(Path.GetDirectoryName(selected), setExportMessage);
                    }
                    if (GUILayout.Button($"EDIT: {pathName}", GUILayout.MinHeight(64)))
                    {
                        onOpen.Invoke(selected);
                    }
                }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
    }
}
