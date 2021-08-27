using System;
using System.Collections.Generic;
using System.IO;

namespace CustomBeatmaps
{
    public static class OsuHelper
    {

        public static bool GetOsuBeatmaps(string path, out string[] beatmapPaths)
        {
            path = GetOsuPath(path);
            if (Directory.Exists(path))
            {
                List<string> beatmaps = new List<string>();
                foreach (string osuProjectDir in Directory.EnumerateDirectories(path))
                {
                    foreach (string file in Directory.EnumerateFiles(osuProjectDir))
                    {
                        if (file.EndsWith(".osu"))
                        {
                            beatmaps.Add(file);
                        }
                    }
                }

                double TimeSinceLastWrite(string filename)
                {
                    return (DateTime.Now - File.GetLastWriteTime(filename)).TotalSeconds;
                }

                // Sort by newest access
                beatmaps.Sort((left, right) => Math.Sign(TimeSinceLastWrite(left) - TimeSinceLastWrite(right)));

                beatmapPaths = beatmaps.ToArray();
                return true;
            }
            beatmapPaths = new string[0];
            return false;
        }

        public static string GetOsuPath(string overridePath)
        {
            if (string.IsNullOrEmpty(overridePath))
            {
                return Path.GetFullPath(Path.Combine(Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData).Replace('\\', '/'), "../Local/osu!/Songs"));
            }
            return overridePath;
        }
    }
}
