using System;
using System.Collections.Generic;
using CustomBeatmaps.Packages;

namespace CustomBeatmaps.UI.Structure
{
    public interface ICustomBeatmapUIMain
    {
        void Init(CustomBeatmapUIMainProps props);
    }

    public struct CustomBeatmapUIMainProps
    {
        public PackageGrabber PackageGrabber;
        public Action<UniqueId, string> OnPlayRequest;
        public Action<UniqueId> OnDownloadRequest;
        public Action<SearchQuery, Action<IList<CustomPackageInfo>>> DoOnlineSearch;
        public Action<SearchQuery, Action<IList<CustomPackageLocalData>>> DoLocalSearch;
        public Action<UniqueId, string, Action<LeaderboardInfo>> DoLeaderboardSearch;
        public Action<Action<int>> GetOnlinePackageCount;
        public Action<Action<int>> GetLocalPackageCount;

        public CustomBeatmapUIMainProps(PackageGrabber packageGrabber, Action<UniqueId, string> onPlayRequest, Action<UniqueId> onDownloadRequest, Action<SearchQuery, Action<IList<CustomPackageInfo>>> doOnlineSearch, Action<SearchQuery, Action<IList<CustomPackageLocalData>>> doLocalSearch, Action<UniqueId, string, Action<LeaderboardInfo>> doLeaderboardSearch, Action<Action<int>> getOnlinePackageCount, Action<Action<int>> getLocalPackageCount)
        {
            PackageGrabber = packageGrabber;
            OnPlayRequest = onPlayRequest;
            OnDownloadRequest = onDownloadRequest;
            DoOnlineSearch = doOnlineSearch;
            DoLocalSearch = doLocalSearch;
            DoLeaderboardSearch = doLeaderboardSearch;
            GetOnlinePackageCount = getOnlinePackageCount;
            GetLocalPackageCount = getLocalPackageCount;
        }
    }
}