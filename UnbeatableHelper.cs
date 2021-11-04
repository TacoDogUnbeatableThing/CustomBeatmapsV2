using CustomBeatmaps.Packages;
using CustomBeatmaps.Patches;

namespace CustomBeatmaps
{
    public static class UnbeatableHelper
    {
        private static readonly string PLAY_SCENE_NAME = "TrainStationRhythm";

        public static void PlayBeatmap(CustomBeatmapInfo beatmap)
        {
            CustomBeatmapLoadingOverridePatch.SetOverrideBeatmap(beatmap);
            OsuEditorPatch.SetEditMode(false);
            LevelManager.LoadLevel(PLAY_SCENE_NAME);
        }

        public static void PlayBeatmapEdit(CustomBeatmapInfo beatmap, string path)
        {
            CustomBeatmapLoadingOverridePatch.SetOverrideBeatmap(beatmap);
            OsuEditorPatch.SetEditMode(true, path);
            LevelManager.LoadLevel(PLAY_SCENE_NAME);
        }

        public static string GetBeatmapUniqueKey(UniqueId id, string difficulty)
        {
            return "[" + id.Id + "::" + difficulty + "]";
        }
    }
}
