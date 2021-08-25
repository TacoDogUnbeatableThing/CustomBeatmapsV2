using CustomBeatmaps.Packages;
using CustomBeatmaps.Patches;

namespace CustomBeatmaps
{
    public static class UnbeatableHelper
    {
        public static void PlayBeatmap(CustomBeatmapInfo beatmap)
        {
            BeatmapParserLoadOverridePatch.SetOverrideBeatmap(beatmap);
            OsuEditorPatch.SetEditMode(false);
            LevelManager.LoadLevel("TEST_RHYTHM");
        }

        public static void PlayBeatmapEdit(CustomBeatmapInfo beatmap, string path)
        {
            BeatmapParserLoadOverridePatch.SetOverrideBeatmap(beatmap);
            OsuEditorPatch.SetEditMode(true, path);
            LevelManager.LoadLevel("TEST_RHYTHM");
        }
    }
}
