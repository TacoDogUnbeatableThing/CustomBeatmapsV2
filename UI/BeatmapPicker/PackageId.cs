using System;

namespace CustomBeatmaps.UI.BeatmapPicker
{
    public struct PackageId
    {
        public string LocalPath;
        public string OnlineIndex;

        public bool IsDownloaded => !string.IsNullOrEmpty(LocalPath);
        public bool IsOnline => !string.IsNullOrEmpty(OnlineIndex);

        public readonly bool Equals(PackageId other)
        {
            return LocalPath == other.LocalPath && OnlineIndex == other.OnlineIndex;
        }

        public override bool Equals(object obj)
        {
            return obj is PackageId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(LocalPath, OnlineIndex);
        }

        public static PackageId FromLocal(string path)
        {
            return new PackageId() {LocalPath = path};
        }

        public static readonly PackageId Empty = new PackageId();
        
    }
}
