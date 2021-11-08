using CustomBeatmaps.UISystem;
using UnityEngine;

namespace CustomBeatmaps.UI.ReactEsque.PackagePreviewUI
{
    public static class BeatmapAssists
    {

        private static int Toggle(int mode, string text)
        {
            return GUILayout.Toggle(mode != 0, text) ? 1 : 0;
        }

        public static void Render()
        {
            // TODO: Kinda kooky but 4 is the magic number.
            (string scrollSpeedText, var setScrollSpeedText) = Reacc.UseState("4");

            JeffBezosController.SetAssistMode(Toggle(JeffBezosController.GetAssistMode(), "Assist Mode"));
            JeffBezosController.SetNoFail(Toggle(JeffBezosController.GetNoFail(), "No Fail"));
            JeffBezosController.SetSongSpeed(GUILayout.Toolbar(JeffBezosController.GetSongSpeed(),
                new[] {"Regular", "Half Time", "Double Time"}));

            GUILayout.BeginHorizontal();
                GUILayout.Label("Scroll Speed:", GUILayout.ExpandWidth(false));
                scrollSpeedText = GUILayout.TextField(scrollSpeedText);
                setScrollSpeedText(scrollSpeedText);
                int spd;
                if (int.TryParse(scrollSpeedText, out spd))
                {
                    JeffBezosController.SetScrollSpeed(spd);
                }
                GUILayout.Label($"= {(float)(JeffBezosController.GetScrollSpeedIndex() + 1) * 0.2f:0.0}");
            GUILayout.EndHorizontal();
        }
    }
}
