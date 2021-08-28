using CustomBeatmaps.Packages;
using CustomBeatmaps.Patches;

namespace CustomBeatmaps
{
    public static class UnbeatableHelper
    {
        public static void PlayBeatmap(CustomBeatmapInfo beatmap)
        {
            CustomBeatmapLoadingOverridePatch.SetOverrideBeatmap(beatmap);
            OsuEditorPatch.SetEditMode(false);
            LevelManager.LoadLevel("TEST_RHYTHM");
        }

        public static void PlayBeatmapEdit(CustomBeatmapInfo beatmap, string path)
        {
            CustomBeatmapLoadingOverridePatch.SetOverrideBeatmap(beatmap);
            OsuEditorPatch.SetEditMode(true, path);
            LevelManager.LoadLevel("TEST_RHYTHM");
        }
    }
}
