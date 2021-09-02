using System.IO;
using UnityEngine;

namespace CustomBeatmaps
{
    public class Settings
    {
        public string OsuSongPathOverride = "";
        public bool DarkMode = true;

        public static Settings Load(string path)
        {
            Settings result;
            if (!File.Exists(path))
            {
                result = new Settings();
            }
            else
            {
                result = JsonUtility.FromJson<Settings>(File.ReadAllText(path));
            }

            return result;
        }

        public static void Save(Settings settings, string path)
        {
            File.WriteAllText(path, JsonUtility.ToJson(settings, true));
        }
    }
}
