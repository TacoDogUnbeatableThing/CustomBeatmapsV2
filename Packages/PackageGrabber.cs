using System;
using System.Collections.Generic;
using System.Linq;
using CustomBeatmaps.UI.Structure;
using UnityEngine;

namespace CustomBeatmaps.Packages
{
    /**
     * A Centralized storage thing to:
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

        public int GetLocalPackageCount()
        {
            EnsureUpdated();
            return _localPackages.Count;
        }

        public List<CustomPackageLocalData> GetLocalPackagesSearched(SearchQuery searchQuery)
        {
            EnsureUpdated();

            // Sort packages by search query and return our segment.
            List<CustomPackageLocalData> list = _localPackages.Values.ToList();
            list.Sort((left, right) => Comparison(left, right, searchQuery.SortType));
            if (!searchQuery.Ascending)
            {
                list.Reverse();
            }

            return list.Skip(searchQuery.StartPackage).Take(searchQuery.EndPackage - searchQuery.StartPackage).ToList();
        }

        private int Comparison(CustomPackageLocalData left, CustomPackageLocalData right, SortType sortType)
        {
            switch (sortType)
            {
                case SortType.Date:
                    return DateTime.Compare(Convert.ToDateTime(left.PackageInfo.Date),
                        Convert.ToDateTime(right.PackageInfo.Date));
                case SortType.Name:
                    return String.CompareOrdinal(left.PackageInfo.PackageName, right.PackageInfo.PackageName);
                default:
                    throw new ArgumentOutOfRangeException(nameof(sortType), sortType, null);
            }
        }

        public PackageDownloadStatus GetDownloadStatus(UniqueId onlineId)
        {
            if (onlineId == null)
            {
                return PackageDownloadStatus.Undefined;
            }
            if (_localPackages.ContainsKey(onlineId) || onlineId.IsPureLocal) return PackageDownloadStatus.Downloaded;
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