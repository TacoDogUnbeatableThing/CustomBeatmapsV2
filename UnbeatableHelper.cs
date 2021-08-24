using CustomBeatmaps.Packages;
using CustomBeatmaps.Patches;

namespace CustomBeatmaps
{
    public static class UnbeatableHelper
    {
        public static void PlayBeatmap(CustomBeatmapInfo beatmap, bool editMode = false)
        {
            BeatmapParserLoadOverridePatch.SetOverrideBeatmap(beatmap);
            LevelManager.LoadLevel("TEST_RHYTHM");
        }
    }
}
