using System;
using System.Collections.Generic;
using CustomBeatmaps.Packages;

namespace CustomBeatmaps.UI.Structure
{
    public interface ICustomBeatmapUIMain
    {
        void Init(CustomBeatmapUIMainProps props);

        void Open();
        void Close();
    }

    public struct CustomBeatmapUIMainProps
    {
        public PackageGrabber PackageGrabber;
        public Action<UniqueId, string> OnPlayRequest;
        public Action<UniqueId> OnDownloadRequest;
        public Action<SearchQuery, Action<IList<CustomPackageInfo>>> DoOnlineSearch;
        public Action<SearchQuery, Action<IList<CustomPackageInfo>>> DoLocalSearch;
        public Action<UniqueId, string, Action<LeaderboardInfo>> DoLeaderboardSearch;
        public Action<Action<int>> GetOnlinePackageCount;
        public Action<Action<int>> GetLocalPackageCount;
        public bool OldBeatmapsDetected;
        public Action<bool, Action<string>> DoConvertOldBeatmaps;
        public Action<Action<string[]>, Action<string>> DoOsuLocalSearch;
        public Action<string> OnEditOsuMap;

        public CustomBeatmapUIMainProps(PackageGrabber packageGrabber, Action<UniqueId, string> onPlayRequest, Action<UniqueId> onDownloadRequest, Action<SearchQuery, Action<IList<CustomPackageInfo>>> doOnlineSearch, Action<SearchQuery, Action<IList<CustomPackageInfo>>> doLocalSearch, Action<UniqueId, string, Action<LeaderboardInfo>> doLeaderboardSearch, Action<Action<int>> getOnlinePackageCount, Action<Action<int>> getLocalPackageCount, bool oldBeatmapsDetected, Action<bool, Action<string>> doConvertOldBeatmaps, Action<Action<string[]>, Action<string>> doOsuLocalSearch, Action<string> onEditOsuMap)
        {
            PackageGrabber = packageGrabber;
            OnPlayRequest = onPlayRequest;
            OnDownloadRequest = onDownloadRequest;
            DoOnlineSearch = doOnlineSearch;
            DoLocalSearch = doLocalSearch;
            DoLeaderboardSearch = doLeaderboardSearch;
            GetOnlinePackageCount = getOnlinePackageCount;
            GetLocalPackageCount = getLocalPackageCount;
            OldBeatmapsDetected = oldBeatmapsDetected;
            DoConvertOldBeatmaps = doConvertOldBeatmaps;
            DoOsuLocalSearch = doOsuLocalSearch;
            OnEditOsuMap = onEditOsuMap;
        }
    }
}
