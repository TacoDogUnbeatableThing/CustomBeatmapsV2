using System.IO;
using Rewired.Utils.Libraries.TinyJson;
using UnityEngine;

namespace CustomBeatmaps.Packages
{
    public readonly struct DifficultyInfo
    {
        public readonly string Name;
        public readonly string Creator;

        public DifficultyInfo(string name, string creator)
        {
            Name = name;
            Creator = creator;
        }
    }

    public readonly struct CustomPackageInfo
    {
        public readonly string PackageName;
        public readonly string Date;
        public readonly DifficultyInfo[] Difficulties;
        public readonly UniqueId DatabaseId;

        public CustomPackageInfo(string packageName, string date, DifficultyInfo[] difficulties,
            UniqueId databaseId)
        {
            PackageName = packageName;
            Date = date;
            Difficulties = difficulties;
            DatabaseId = databaseId;
        }

        public static CustomPackageInfo Load(string fpath)
        {
            string text = File.ReadAllText(fpath);
            CustomPackageInfo result = JsonParser.FromJson<CustomPackageInfo>(text);
            return result;
        }

        public static void Save(CustomPackageInfo data, string fpath)
        {
            File.WriteAllText(fpath, JsonWriter.ToJson(data));
        }
    }
}