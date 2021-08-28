using CustomBeatmaps.Packages;
using CustomBeatmaps.UISystem;
using UnityEngine;

namespace CustomBeatmaps.UI.ReactEsque.PackagePreviewUI
{
    public static class PersonalHighScore
    {
        public static void Render(UniqueId selectedBeatmap, string selectedDifficulty)
        {
            (HighScoreList highScores, var setHighScores) =
                Reacc.UseState(() => HighScoreScreen.LoadHighScores("wl-highscores"));

            string key = UnbeatableHelper.GetBeatmapUniqueKey(selectedBeatmap, selectedDifficulty);
            var score = highScores.GetScoreItem(key);
            GUILayout.Label($"<color=green>{score.score:00000000}</color> PTS, <color=blue>{(int)(score.accuracy*100f)}%</color>");
        }
    }
}
