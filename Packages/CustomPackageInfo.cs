using System.IO;
using Rewired.Utils.Libraries.TinyJson;

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
        public readonly string Artist;
        public readonly DifficultyInfo[] Difficulties;
        public readonly UniqueId DatabaseId;
        public readonly string FileId;

        public CustomPackageInfo(string packageName, string date, string artist, DifficultyInfo[] difficulties,
            UniqueId databaseId, string fileId)
        {
            PackageName = packageName;
            Date = date;
            Artist = artist;
            Difficulties = difficulties;
            DatabaseId = databaseId;
            FileId = fileId;
        }

        public static CustomPackageInfo Load(string fpath)
        {
            return JsonParser.FromJson<CustomPackageInfo>(File.ReadAllText(fpath));
        }

        public static void Save(CustomPackageInfo data, string fpath)
        {
            File.WriteAllText(fpath, JsonWriter.ToJson(data));
        }
    }
}