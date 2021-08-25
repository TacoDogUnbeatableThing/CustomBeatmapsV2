using CustomBeatmaps.Packages;
using HarmonyLib;
using Rhythm;
using UnityEngine;

namespace CustomBeatmaps.Patches
{
    public static class BeatmapInfoAudioKeyOverridePatch
    {
        [HarmonyPatch(typeof(BeatmapInfo), "audioKey", MethodType.Getter)]
        [HarmonyPrefix]
        private static void BeatmapInfoAudioKey(BeatmapInfo __instance, ref string __result, ref bool __runOriginal)
        {
            if (__instance is CustomBeatmapInfo)
            {
                __result = ((CustomBeatmapInfo) __instance).RealAudioKey;
                Debug.Log($"FOUND CUSTOM: New Audio Path: {__result}");
                __runOriginal = false;
            }
        }
    }
}
