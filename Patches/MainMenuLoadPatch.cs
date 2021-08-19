using System;
using HarmonyLib;

namespace CustomBeatmaps.Patches
{
    public static class MainMenuLoadPatch
    {
        public static Action OnOpen;

        [HarmonyPatch(typeof(WhiteLabelMainMenu), "Start")]
        [HarmonyPostfix]
        static void OnMainMenuLoad()
        {
            OnOpen?.Invoke();
        }
    }
}