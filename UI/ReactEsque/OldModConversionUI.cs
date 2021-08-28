using System;
using CustomBeatmaps.UISystem;
using UnityEngine;

namespace CustomBeatmaps.UI.ReactEsque
{
    public static class OldModConversionUI
    {

        public static void Render(Action<bool, Action<string>> onRun, Action onClose)
        {
            float windowWidth = Math.Max(Screen.width / 3, 256);
            float windowHeight = Math.Min(Screen.height, 512);

            (bool forceMove, var setForceMove) = Reacc.UseState(true);
            (string message, var setMessage) = Reacc.UseState("");
            (Vector2 textScroll, var setTextScroll) = Reacc.UseState(Vector2.zero);

            Rect centerRect = new Rect(Screen.width/2f - windowWidth/2, Screen.height/2f - windowHeight, windowWidth, windowHeight);
            GUILayout.Window(Reacc.GetUniqueId(), centerRect, _ =>
            {
                bool hasMessage = !string.IsNullOrEmpty(message);
                GUILayout.Label("You may convert beatmaps from the old mod here.");
                GUILayout.Space(16);
                if (hasMessage)
                {
                    setTextScroll(GUILayout.BeginScrollView(textScroll));
                        GUILayout.TextArea(message);
                    GUILayout.EndScrollView();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Close", GUILayout.ExpandWidth(true)))
                    {
                        onClose.Invoke();
                    }
                }
                else
                {
                    GUILayout.FlexibleSpace();
                    setForceMove(GUILayout.Toggle(forceMove, "Don't Copy (Just Move)", GUILayout.ExpandWidth(true)));
                    if (GUILayout.Button("CONVERT", GUILayout.ExpandWidth(true)))
                    {
                        setMessage("...Converting...\n");
                        onRun.Invoke(forceMove, log =>
                        {
                            // Kinda jank?
                            message += log + "\n";
                            setMessage(message);
                        });
                    }
                }
            }, "Old Beatmap Converter");
        }
    }
}
