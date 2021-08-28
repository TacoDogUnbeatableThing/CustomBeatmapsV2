using CustomBeatmaps.Packages;
using CustomBeatmaps.Patches;

namespace CustomBeatmaps
{
    public static class UnbeatableHelper
    {
        public static void PlayBeatmap(CustomBeatmapInfo beatmap)
        {
            CustomBeatmapLoadingOverridePatch.SetOverrideBeatmap(beatmap);
            LevelManager.LoadLevel("TEST_RHYTHM");
        }

        public static string GetBeatmapUniqueKey(UniqueId id, string difficulty)
        {
            return "[" + id.Id + "::" + difficulty + "]";
        }
    }
}
