using System;
using System.Linq;
using CustomBeatmaps.Packages;
using UnityEngine;

namespace CustomBeatmaps.UI.BeatmapPicker
{
    public static class PackagePreview
    {
        public static void Renderer(PackageId packageId, Action<string> onPlay)
        {
            CustomPackageStrippedInfo package =
                CustomBeatmaps.Instance.PackageGrabber.GetCustomPackageStrippedInfo(packageId);

            RenderProper(
                package.PackageName,
                package.Artists,
                package.Difficulties,
                package.BeatmapCreators,
                CustomBeatmaps.Instance.PackageGrabber.GetPackageDownloadStatus(packageId),
                () => CustomBeatmaps.Instance.PackageGrabber.RequestDownload(packageId),
                onPlay
            );
        }

        private static void RenderProper(string packageName, string[] artists, string[] difficulties,
            string[] beatmapCreators, PackageDownloadStatus status, Action onDownload, Action<string> onPlay)
        {
            (int selectedDifficulty, var setSelectedDifficulty) = UI.UseState(0);
            GUILayout.BeginHorizontal();
                // TODO: Texture formatting
                GUILayout.Label(Texture2D.linearGrayTexture, GUILayout.Width(64), GUILayout.MaxHeight(64));
                GUILayout.BeginVertical();
                    GUIStyle wrappingStyle = new GUIStyle(GUI.skin.label);
                    wrappingStyle.wordWrap = true;
                    GUILayout.Label($"<b>{packageName}</b>");
                    GUILayout.Label($"By {string.Join(", ", artists)}", wrappingStyle, GUILayout.MaxHeight(64));
                    GUILayout.Label($"Difficulties: {string.Join(", ", difficulties)}", wrappingStyle, GUILayout.MaxHeight(64));
                    GUILayout.Label($"Creators: {string.Join(", ", beatmapCreators)}", wrappingStyle, GUILayout.MaxHeight(64));

                    GUILayout.BeginHorizontal();
                        // TODO: Dropdown?
                        GUILayout.Label("Difficulty:");
                        setSelectedDifficulty(GUILayout.Toolbar(selectedDifficulty, difficulties));
                    GUILayout.EndHorizontal();
                    
                    GUILayout.Label("TODO: Leaderboards go here");

                    switch (status)
                    {
                        case PackageDownloadStatus.Online:
                            if (GUILayout.Button("DOWNLOAD"))
                            {
                                onDownload();
                            }
                            break;
                        case PackageDownloadStatus.Downloading:
                            GUILayout.Label("Downloading...");
                            break;
                        case PackageDownloadStatus.Downloaded:
                            if (GUILayout.Button("PLAY"))
                            {
                                onPlay(difficulties[selectedDifficulty]);
                            }
                            break;
                    }
                    
                GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }
}
