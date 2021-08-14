using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CustomBeatmaps.Packages
{
    // Some Design Credits to Ratismal's CustomBeats Plugin
    public class PackageGrabberUtils
    {
        private static readonly string INFO_JSON_NAME = "info.json";

        public static Dictionary<UniqueId, CustomPackageLocalData> LoadLocalCustomPackages(string folderPath)
        {
            var result = new Dictionary<UniqueId, CustomPackageLocalData>();

            if (!Directory.Exists(folderPath))
                // No directory found.
                return result;
            // Each directory is 1 extracted package.
            foreach (var packageDir in Directory.GetDirectories(folderPath))
            {
                // A package may have multiple beatmaps.
                var beatmaps = new List<CustomBeatmapInfo>();
                foreach (var fname in Directory.GetFiles(packageDir, "*.osu"))
                {
                    var osuFile = fname.Replace("\\", "/");
                    try
                    {
                        Debug.Log($"Loading Local Package: {osuFile}");
                        var text = File.ReadAllText(osuFile);
                        var beatmap = LoadBeatmap(osuFile, text);
                        beatmaps.Add(beatmap);
                    }
                    catch (BeatmapLoadException e)
                    {
                        CustomBeatmaps.Instance.ShowError(e);
                    }
                }

                // No beatmaps = no package.
                if (beatmaps.Count != 0)
                {
                    var packageId = Path.GetFileNameWithoutExtension(packageDir);
                    // Get the id from folder name. Used to reference online stuff.
                    var id = new UniqueId(packageId);
                    result.Add(id, new CustomPackageLocalData(LoadPackageInfo(packageDir), beatmaps));
                }
            }

            return result;
        }

        private static CustomPackageInfo LoadPackageInfo(string packageDir)
        {
            var jsonFile = Path.Join(packageDir, INFO_JSON_NAME);
            return CustomPackageInfo.Load(jsonFile);
        }

        private static bool TryGetProp(string text, string prop, out string result)
        {
            var match = Regex.Match(text, $"{prop}: *(.+?)\r?\n");
            if (match.Groups.Count > 1)
            {
                result = match.Groups[1].Value;
                return true;
            }

            result = null;
            return false;
        }

        private static CustomBeatmapInfo LoadBeatmap(string path, string text)
        {
            string songName, difficulty, artist, beatmapCreator, audioFile;
            if (!TryGetProp(text, "Title", out songName))
                throw new BeatmapLoadException(path, "Title property not found.");
            if (!TryGetProp(text, "Version", out difficulty))
                throw new BeatmapLoadException(path, "Difficulty property not found.");
            if (!TryGetProp(text, "Artist", out artist))
                throw new BeatmapLoadException(path, "Author property not found.");
            if (!TryGetProp(text, "Creator", out beatmapCreator))
                throw new BeatmapLoadException(path, "Creator property not found.");
            if (!TryGetProp(text, "AudioFilename", out audioFile))
                throw new BeatmapLoadException(path, "AudioFilename property not found.");

            var relPath = Path.GetDirectoryName(path);
            var trueAudioPath = Path.Join(relPath, audioFile);

            return new CustomBeatmapInfo(new TextAsset(text), songName, difficulty, artist, beatmapCreator,
                trueAudioPath);
        }

        public static void ListenToLocalChanges(string rootDirectory, Action onChange)
        {
            // When any file within the folder gets updated...
            try
            {
                var fileWatcher = new FileSystemWatcher
                {
                    Path = rootDirectory,
                    NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName |
                                   NotifyFilters.FileName
                                   | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size,
                    EnableRaisingEvents = true
                };
                fileWatcher.IncludeSubdirectories = true;
                fileWatcher.Changed += (sender, args) => onChange.Invoke();
                fileWatcher.Created += (sender, args) => onChange.Invoke();
            }
            catch (ArgumentException e)
            {
                CustomBeatmaps.Instance.ShowError(e);
            }
        }


        private class BeatmapLoadException : Exception
        {
            public BeatmapLoadException(string path, string message) : base($"Failed to load {path}: {message}")
            {
            }
        }
    }
}