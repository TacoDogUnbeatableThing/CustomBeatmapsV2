using CustomBeatmaps.Packages;

namespace CustomBeatmaps.UI.BeatmapPicker
{
    public class CustomPackageStrippedInfo
    {
        public readonly PackageId PackageId;
        public readonly string PackageName;
        public readonly string[] Artists;
        public readonly string[] BeatmapCreators;
        public readonly string[] Difficulties;

        public CustomPackageStrippedInfo(PackageId packageId, string packageName, string[] artists, string[] beatmapCreators, string[] difficulties)
        {
            PackageId = packageId;
            PackageName = packageName;
            Artists = artists;
            BeatmapCreators = beatmapCreators;
            Difficulties = difficulties;
        }
        public CustomPackageStrippedInfo(CustomPackageInfo packageInfo) : this(packageInfo.PackageId, packageInfo.PackageName, packageInfo.Artists, packageInfo.BeatmapCreators, packageInfo.Difficulties) {}
    }
}