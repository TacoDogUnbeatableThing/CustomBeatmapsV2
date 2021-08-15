using System;
using System.Linq;
using CustomBeatmaps.Packages;
using CustomBeatmaps.UI.Structure;
using CustomBeatmaps.UISystem;
using UnityEngine;

namespace CustomBeatmaps.UI.ReactEsque.PackagePreviewUI
{
    public static class PackagePreview
    {
        public static void Render(
            CustomPackageInfo packageInfo,
            PackageDownloadStatus downloadStatus,
            Action<string, Action<LeaderboardInfo>> doLeaderboardSearch,
            Action onDownloadRequested,
            Action<string> onDifficultyPlayRequested,
            params GUILayoutOption[] options)
        {

            string GetDefaultDifficulty()
            {
                return packageInfo.difficulties.Count != 0
                    ? packageInfo.difficulties.Keys.First()
                    : "(none found)";
            }

            (string selectedDifficulty, var setSelectedDifficulty) = Reacc.UseState(
                GetDefaultDifficulty
                );
            (LeaderboardInfo currentLeaderboard, var setCurrentLeaderboard) = Reacc.UseState(LeaderboardInfo.Empty);
            (bool loadingLeaderboard, var setLoadingLeaderboard) = Reacc.UseState(true);

            bool hasOnline = !packageInfo.DatabaseId.IsPureLocal; 

            Reacc.UseEffect(() =>
            {
                if (hasOnline)
                {
                    // Load leaderboards when we change difficulties
                    setLoadingLeaderboard(true);
                    doLeaderboardSearch.Invoke(selectedDifficulty, leaderboard =>
                    {
                        setCurrentLeaderboard(leaderboard);
                        setLoadingLeaderboard(false);
                    });
                }
                else
                {
                    // We're local, don't try loading difficulties.
                    setLoadingLeaderboard(false);
                }
            }, new object[]{selectedDifficulty});
            Reacc.UseEffect(() =>
            {
                // On package change, update difficulty selection.
                setSelectedDifficulty.Invoke(GetDefaultDifficulty());
            }, new object[] {packageInfo.DatabaseId, packageInfo.difficulties.Count});

            /*
             * Package info header
             * IF online: Leaderboard area (with "loading..." or "none found")
             * Get download status
             * If downloading:
             *      Text that says "downloading..."
             * If online only:
             *      Button that says "Download"
             * If downloaded:
             *      Button that says "Play"
             */
            // Package Header
            GUILayout.BeginVertical(options);
                PackageHeader.Render(packageInfo);
                // Difficulty Picker
                GUILayout.BeginHorizontal();
                    GUILayout.Label("Difficulty: ");
                    Dropdown.Render(
                        selectedDifficulty,
                        packageInfo.difficulties.Keys.ToList(),
                        index => setSelectedDifficulty(packageInfo.difficulties.Keys.ToList()[index]),
                        GUILayout.Width(128));
                GUILayout.EndHorizontal();
                // Leaderboard
                PackageLeaderboard.Render(currentLeaderboard.GetRanks(selectedDifficulty));

                GUILayout.FlexibleSpace();

                // Buttons (play & download)
                switch (downloadStatus)
                {
                    case PackageDownloadStatus.OnlineOnly:
                        if (BottomButton("DOWNLOAD"))
                        {
                            onDownloadRequested.Invoke();
                        }
                        break;
                    case PackageDownloadStatus.Downloading:
                        AnimatedDownloadText.Render();
                        break;
                    case PackageDownloadStatus.Downloaded:
                        if (BottomButton("PLAY"))
                        {
                            onDifficultyPlayRequested.Invoke(selectedDifficulty);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(downloadStatus), downloadStatus, null);
                }
            GUILayout.EndVertical();
        }

        private static bool BottomButton(string text)
        {
            // Might add some funky formatting later to make this BIG
            return GUILayout.Button(text, GUILayout.ExpandWidth(true), GUILayout.MinHeight(64));
        }
    }
}
