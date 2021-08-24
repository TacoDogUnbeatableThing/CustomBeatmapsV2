using System.IO;
using UnityEngine;

namespace CustomBeatmaps
{
    public class Settings
    {
        public string OsuSongPathOverride = "";

        public static Settings Load(string path)
        {
            Settings result;
            if (!File.Exists(path))
            {
                result = new Settings();
                File.WriteAllText(path, JsonUtility.ToJson(result, true));
            }
            else
            {
                result = JsonUtility.FromJson<Settings>(File.ReadAllText(path));
            }

            return result;
        }
    }
}
