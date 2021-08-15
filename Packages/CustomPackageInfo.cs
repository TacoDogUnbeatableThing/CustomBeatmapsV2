using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Rewired.Utils.Libraries.TinyJson;

namespace CustomBeatmaps.Packages
{
    // Naming matches database/file naming.
    // I could use external JSON libraries to rename these, but eh
    // this is easy for now.
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public readonly struct CustomPackageInfo
    {
        public readonly string name;
        public readonly string date;
        public readonly string artist;
        public readonly Dictionary<string, string> difficulties;
        public readonly UniqueId DatabaseId;

        public CustomPackageInfo(string name, string date, string artist, Dictionary<string, string> difficulties,
            UniqueId databaseId)
        {
            this.name = name;
            this.date = date;
            this.artist = artist;
            this.difficulties = difficulties;
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