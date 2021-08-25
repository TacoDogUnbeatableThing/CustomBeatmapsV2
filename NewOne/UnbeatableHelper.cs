using CustomBeatmaps.Packages;
using CustomBeatmaps.Patches;

namespace CustomBeatmaps
{
    public static class UnbeatableHelper
    {
        public static void PlayBeatmap(CustomBeatmapInfo beatmap)
        {
            BeatmapParserLoadOverridePatch.SetOverrideBeatmap(beatmap);
            LevelManager.LoadLevel("TEST_RHYTHM");
        }
    }
}