using System.Collections.Generic;

namespace CustomBeatmaps.Packages
{
    public class CustomPackageLocalData
    {
        public readonly List<CustomBeatmapInfo> Beatmaps;
        public readonly CustomPackageInfo PackageInfo;

        public CustomPackageLocalData(CustomPackageInfo packageInfo, List<CustomBeatmapInfo> beatmaps)
        {
            PackageInfo = packageInfo;
            Beatmaps = beatmaps;
        }
    }
}