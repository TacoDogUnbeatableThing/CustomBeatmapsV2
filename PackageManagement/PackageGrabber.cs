using System;
using System.Collections.Generic;
using System.IO;
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

        public bool LocalOutOfSync => !_updated;

        public PackageGrabber(string localPackagesDirectory)
        {
            _localPackagesDirectory = localPackagesDirectory;
            if (!Directory.Exists(localPackagesDirectory))
            {
                Directory.CreateDirectory(localPackagesDirectory);
            }
            // If any file gets updated locally, mark the local cache as dirty.
            PackageGrabberUtils.ListenToLocalChanges(_localPackagesDirectory, () =>
            {
                Console.WriteLine("Local Packages CHANGE DETECTED!");
                _updated = false;
            });
        }

        public int GetLocalPackageCount()
        {
            EnsureUpdated();
            return _localPackages.Count;
        }

        public List<CustomPackageInfo> GetLocalPackagesSearched(SearchQuery searchQuery)
        {
            EnsureUpdated();

            // Sort packages by search query and return our segment.
            return GetListPackagesSearched(_localPackages.Values.Select(package => package.PackageInfo).ToList(), searchQuery);
        }

        private List<CustomPackageInfo> GetListPackagesSearched(List<CustomPackageInfo> everything,
            SearchQuery searchQuery)
        {
            everything.Sort((left, right) => Comparison(left, right, searchQuery.SortType));
            if (!searchQuery.Ascending)
            {
                everything.Reverse();
            }

            return everything.Skip(searchQuery.StartPackage).Take(searchQuery.EndPackage - searchQuery.StartPackage).ToList();
        }

        private int Comparison(CustomPackageInfo left, CustomPackageInfo right, SortType sortType)
        {
            switch (sortType)
            {
                case SortType.Date:
                    return DateTime.Compare(Convert.ToDateTime(left.date),
                        Convert.ToDateTime(right.date));
                case SortType.Name:
                    return String.CompareOrdinal(left.name, right.name);
                default:
                    throw new ArgumentOutOfRangeException(nameof(sortType), sortType, null);
            }
        }

        public CustomBeatmapInfo GetLocalBeatmap(UniqueId id, string difficulty)
        {
            EnsureUpdated();
            if (_localPackages.ContainsKey(id))
            {
                CustomPackageLocalData dat = _localPackages[id];
                return dat.Beatmaps.Find(info => info.difficulty.Equals(difficulty));
            }
            return null;
        }

        public PackageDownloadStatus GetDownloadStatus(UniqueId onlineId)
        {
            EnsureUpdated();
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
