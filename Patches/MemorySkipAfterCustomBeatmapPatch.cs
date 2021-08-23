using HarmonyLib;
using UnityEngine;

namespace CustomBeatmaps.Patches
{
    public static class MemorySkipAfterCustomBeatmapPatch
    {
        // Makes sure we don't crash after completing a beatmap.
        [HarmonyPatch(typeof(GetMemory), "Start")]
        [HarmonyPrefix]
        static void MemoryStartPatch(ref bool __runOriginal, ref bool ___skip)
        {
            if (JeffBezosController.beatmapToLoad == null)
            {
                Debug.Log("No loaded beatmap, assuming custom and going back to main menu.");
                ___skip = true;
                __runOriginal = false;
            }
        }
    }
}
