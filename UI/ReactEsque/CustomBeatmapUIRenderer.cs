﻿using System;
using System.Collections.Generic;
using System.Linq;
using CustomBeatmaps.Packages;
using CustomBeatmaps.UI.ReactEsque.PackageListUI;
using CustomBeatmaps.UI.ReactEsque.PackagePreviewUI;
using CustomBeatmaps.UI.Structure;
using CustomBeatmaps.UISystem;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomBeatmaps.UI.ReactEsque
{
    public class CustomBeatmapUIRenderer : UIRenderer, ICustomBeatmapUIMain
    {
        private CustomBeatmapUIMainProps _props;
        public void Init(CustomBeatmapUIMainProps props)
        {
            _props = props;
            Object.DontDestroyOnLoad(GameObject);
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

            // State
            (bool online, var setOnline) = Reacc.UseState(false);
            (int pageSize, var setPageSize) = Reacc.UseState(CalculatePageSize);
            (int totalPackages, var setTotalPackages) = Reacc.UseState(0);
            (SearchQuery searchQuery, var setSearchQuery) = Reacc.UseState(() => new SearchQuery("", true, SortType.Date, 0, pageSize));
            (IList<CustomPackageInfo> packageInfos, var setPackageInfos) =
                Reacc.UseState(() => new List<CustomPackageInfo>());
            (int selectedPackage, var setSelectedPackage) = Reacc.UseState(-1);
            (bool loading, var setLoading) = Reacc.UseState(true);

            bool packageSelected = selectedPackage != -1;
            
            // (On Packages Loaded)
            void SetPackageInfoLoaded(ICollection<CustomPackageInfo> data)
            {
                setPackageInfos.Invoke((List<CustomPackageInfo>) data);
                setSelectedPackage(-1);
                setLoading(false);
                Debug.Log($"Loaded in {data.Count} Beatmaps");
            }

            // Effects
            Reacc.UseEffect(() =>
            {
                // When our query changes or we switch modes, reload.
                Debug.Log($"RELOADING BEATMAPS: {(online? "ONLINE" : "LOCAL")}");
                setLoading(true);
                if (online)
                {
                    _props.DoOnlineSearch.Invoke(searchQuery, SetPackageInfoLoaded);
                }
                else
                {
                    _props.DoLocalSearch.Invoke(searchQuery, localList =>
                    {
                        SetPackageInfoLoaded(localList.Select(local => local.PackageInfo).ToList());
                    });
                }
            }, new object[]{searchQuery, online});
            Reacc.UseEffect(() =>
            {
                // When we switch from online mode to offline mode, reload our total "count"
                if (online)
                {
                    _props.GetOnlinePackageCount.Invoke(setTotalPackages);
                }
                else
                {
                    _props.GetLocalPackageCount.Invoke(setTotalPackages);
                }
            }, new object[]{online});

            // UI
            int windowPad = 16;
            Rect centerRect = new Rect(windowPad, windowPad, Screen.width - windowPad*2, Screen.height - windowPad*2);
            GUILayout.Window(Reacc.GetUniqueId(), centerRect, _ =>
            {
                OnlinePicker.Render(online, setOnline);
                SearchBar.Render(searchQuery, setSearchQuery);
                GUILayout.BeginHorizontal();
                    // Package List
                    int pageNumber = (searchQuery.StartPackage / pageSize) + 1;
                    int totalPages = (totalPackages / pageSize) + 1;
                    if (totalPackages == 0)
                    {
                        pageNumber = 0;
                        totalPages = 0;
                    }
                    PackageListPicker.Render(
                        packageInfos,
                        selectedPackage,
                        setSelectedPackage,
                        pageNumber,
                        totalPages,
                        newPage =>
                        {
                            // On new page, change our search query.
                            int start = newPage * pageSize;
                            int end = start + pageSize;
                            setSearchQuery.Invoke(new SearchQuery(searchQuery.TextQuery, searchQuery.Ascending, searchQuery.SortType, start, end));
                        });
                    // Package Preview
                    if (packageSelected)
                    {
                        CustomPackageInfo currentPackage = packageInfos[selectedPackage];
                        PackagePreview.Render(
                            currentPackage,
                            _props.PackageGrabber.GetDownloadStatus(currentPackage.DatabaseId),
                            (difficultyRequested, onGrab) =>
                                _props.DoLeaderboardSearch.Invoke(currentPackage.DatabaseId, difficultyRequested, onGrab),
                            () => _props.OnDownloadRequest.Invoke(currentPackage.DatabaseId),
                            difficultyToPlay => _props.OnPlayRequest.Invoke(currentPackage.DatabaseId, difficultyToPlay),
                            GUILayout.Width(Screen.width / 2 - windowPad*2)
                            );
                    }
                GUILayout.EndHorizontal();
            }, "Beatmap Picker");
        }

        private int CalculatePageSize()
        {
            // TODO: Depending on package list size?
            return 50;
        }
    }
}