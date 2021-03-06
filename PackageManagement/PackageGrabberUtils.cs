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
                        var beatmap = LoadBeatmap(osuFile);
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
                    try
                    {
                        var packageId = Path.GetFileNameWithoutExtension(packageDir);
                        // Get the id from folder name. Used to reference online stuff.
                        var id = new UniqueId(packageId);
                        CustomPackageInfo packageInfo = LoadPackageInfo(packageDir);
                        // Copy over id from folder name
                        packageInfo = new CustomPackageInfo(packageInfo.name, packageInfo.date, packageInfo.artist,
                            packageInfo.difficulties, id);
                        result.Add(id, new CustomPackageLocalData(packageInfo, beatmaps));
                    }
                    catch (Exception e)
                    {
                        CustomBeatmaps.Instance.ShowError(e);
                    }
                }
            }

            return result;
        }

        private static CustomPackageInfo LoadPackageInfo(string packageDir)
        {
            var jsonFile = packageDir + "/" + INFO_JSON_NAME; // Path.Join fails.
            return CustomPackageInfo.Load(jsonFile);
        }

        public static string GetBeatmapProp(string beatmapText, string prop, string path)
        {
            var match = Regex.Match(beatmapText, $"{prop}: *(.+?)\r?\n");
            if (match.Groups.Count > 1)
            {
                return match.Groups[1].Value;
            }
            throw new BeatmapLoadException(path, $"{prop} property not found.");
        }

        public static CustomBeatmapInfo LoadBeatmap(string bmapPath)
        {
            string text = File.ReadAllText(bmapPath);
            string songName = GetBeatmapProp(text, "Title", bmapPath);
            string difficulty = GetBeatmapProp(text, "Version", bmapPath);
            string artist = GetBeatmapProp(text, "Artist", bmapPath);
            string beatmapCreator = GetBeatmapProp(text, "Creator", bmapPath);
            string audioFile = GetBeatmapProp(text, "AudioFilename", bmapPath);


            var audioFolder = Path.GetDirectoryName(bmapPath);
            var trueAudioPath = audioFolder + "/" + audioFile; // Path.Join fails.

            return new CustomBeatmapInfo(new TextAsset(text), difficulty, artist, beatmapCreator,
                songName, trueAudioPath);
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
                    EnableRaisingEvents = true,
                    Filter = "*",
                    IncludeSubdirectories = true
                };
                fileWatcher.Changed += (sender, args) => onChange.Invoke();
                fileWatcher.Created += (sender, args) => onChange.Invoke();
                fileWatcher.Deleted += (sender, args) => onChange.Invoke();
                fileWatcher.Renamed += (sender, args) => onChange.Invoke();
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
