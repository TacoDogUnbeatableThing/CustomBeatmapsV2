using CustomBeatmaps.Packages;
using HarmonyLib;
using Rhythm;
using UnityEngine;

namespace CustomBeatmaps.Patches
{
    public static class BeatmapParserLoadOverridePatch
    {

        private static CustomBeatmapInfo _override;

        public static void SetOverrideBeatmap(CustomBeatmapInfo toOverride)
        {
            _override = toOverride;
        }

        public static void ResetOverrideBeatmap()
        {
            _override = null;
        }

        [HarmonyPatch(typeof(BeatmapParser), "ParseBeatmap")]
        [HarmonyPrefix]
        private static void ParseBeatmap(BeatmapParser __instance, ref bool __runOriginal)
        {
            if (_override != null)
            {
                __runOriginal = false;
                BeatmapParserEngine beatmapParserEngine = new BeatmapParserEngine();
                __instance.beatmap = ScriptableObject.CreateInstance<Beatmap>();
                var beatmapInfo = _override;
                beatmapParserEngine.ReadBeatmap(beatmapInfo.text, ref __instance.beatmap);
                __instance.audioKey = beatmapInfo.audioKey;
            }
        }
    }
}
