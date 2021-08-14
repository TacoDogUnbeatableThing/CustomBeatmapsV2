using System.Collections.Generic;

namespace CustomBeatmaps.Packages
{
    /**
     * TODO: Will be a centralized storage thing to:
     * - Hang on to local packages
     * - Update automatically when there are local changes
     * - Query server for available online packages
     * - Download packages
     */
    public class PackageGrabber
    {
        private readonly string _localPackagesDirectory;

        private Dictionary<UniqueId, CustomPackageLocalData> _localPackages =
            new Dictionary<UniqueId, CustomPackageLocalData>();

        private bool _updated;

        public PackageGrabber(string localPackagesDirectory)
        {
            _localPackagesDirectory = localPackagesDirectory;
            // If any file gets updated locally, mark the local cache as dirty.
            PackageGrabberUtils.ListenToLocalChanges(_localPackagesDirectory, () => _updated = false);
        }

        public Dictionary<UniqueId, CustomPackageLocalData> GetLocalPackages()
        {
            EnsureUpdated();
            return _localPackages;
        }

        public PackageDownloadStatus GetDownloadStatus(UniqueId onlineId)
        {
            if (_localPackages.ContainsKey(onlineId)) return PackageDownloadStatus.Downloaded;
            // TODO: Consult a Downloader for download status.
            return PackageDownloadStatus.OnlineOnly;
        }

        private void EnsureUpdated()
        {
            if (!_updated) ReloadLocalPackages();
        }

        private void ReloadLocalPackages()
        {
            _localPackages = PackageGrabberUtils.LoadLocalCustomPackages(_localPackagesDirectory);
            _updated = true;
        }
    }
}