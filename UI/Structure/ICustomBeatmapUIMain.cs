using System;
using System.Collections.Generic;
using CustomBeatmaps.Packages;

namespace CustomBeatmaps.UI.Structure
{
    public interface ICustomBeatmapUIMain
    {
        void Init(PackageGrabber packageGrabber, Action<CustomBeatmapInfo> onPlayRequest, Action<UniqueId> onDownloadRequest, Action<SearchQuery, Action<ICollection<CustomPackageInfo>>> doOnlineSearch, Action<SearchQuery, Action<ICollection<CustomPackageLocalData>>> doLocalSearch);
    }
}
