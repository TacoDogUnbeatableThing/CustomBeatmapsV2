using System;
using System.Collections.Generic;
using CustomBeatmaps.Packages;
using CustomBeatmaps.UI.Structure;
using CustomBeatmaps.UISystem;

namespace CustomBeatmaps.UI.ReactEsque
{
    public class CustomBeatmapUIRenderer : UIRenderer, ICustomBeatmapUIMain
    {
        public void Init(PackageGrabber packageGrabber, Action<CustomBeatmapInfo> onPlayRequest, Action<UniqueId> onDownloadRequest, Action<SearchQuery, Action<ICollection<CustomPackageInfo>>> doOnlineSearch, Action<SearchQuery, Action<ICollection<CustomPackageLocalData>>> doLocalSearch)
        {
            // TODO: Store locally/constructor
            throw new NotImplementedException();
        }

        protected override void OnUnityGUI()
        {
            // TODO: Render shit.
            /*
             * State:
             *  - Local/Online mode
             *  - Current search query
             *  - List of package infos
             *  - Selected package index
             * Effect:
             *  - Search Query+Online Mode -> Do Search (online or offline)
             * Render:
             *  - Online mode/Offline mode picker
             *  - Search bar (pass+change text + sort)
             *  - Package Picker (send & get package index)
             *  - Package Viewer (send package info[package index] along with OnPlay + OnDownload)
             */
        }
    }
}
