using System;
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
        private ReaccStore _store = new ReaccStore();
        public void Init(CustomBeatmapUIMainProps props)
        {
            _props = props;
            GameObject.SetActive(false);
        }

        public void Open()
        {
            GameObject.SetActive(true);
        }

        public void Close()
        {
            if (GameObject.activeSelf)
            {
                GameObject.SetActive(false);
                // Refresh UI
                _store = new ReaccStore();
            }
        }

        protected override void OnUnityGUI()
        {
            Reacc.SetStore(_store);
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
            (bool open, var setOpen) = Reacc.UseState(false);
            (PackageMode packageMode, var setPackageMode) = Reacc.UseState(PackageMode.Local);
            (int pageSize, var setPageSize) = Reacc.UseState(CalculatePageSize);
            (int totalPackages, var setTotalPackages) = Reacc.UseState(0);
            (SearchQuery searchQuery, var setSearchQuery) = Reacc.UseState(() => new SearchQuery("", true, SortType.Date, 0, pageSize));
            (IList<CustomPackageInfo> packageInfos, var setPackageInfos) =
                Reacc.UseState(() => new List<CustomPackageInfo>());
            (int selectedPackage, var setSelectedPackage) = Reacc.UseState(-1);
            (bool loading, var setLoading) = Reacc.UseState(true);
            (bool oldMapConversionOpen, var setOldMapConversionOpen) = Reacc.UseState(false);
            (bool oldMapsDetected, var setOldMapsDetected) = Reacc.UseState(_props.OldBeatmapsDetected);
            (string[] osuMaps, var setOsuMaps) = Reacc.UseState(new string[0]);
            (string osuError, var setOsuError) = Reacc.UseState("");

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
                Debug.Log($"RELOADING BEATMAPS: {packageMode}");
                setLoading(true);
                if (packageMode == PackageMode.Online)
                {
                    _props.DoOnlineSearch.Invoke(searchQuery, SetPackageInfoLoaded);
                }
                else if (packageMode == PackageMode.Local)
                {
                    _props.DoLocalSearch.Invoke(searchQuery, SetPackageInfoLoaded);
                }
            }, new object[]{searchQuery, packageMode});
            Reacc.UseEffect(() =>
            {
                // When we switch from online mode to offline mode, reload our total "count"
                if (packageMode == PackageMode.Online)
                {
                    _props.GetOnlinePackageCount.Invoke(setTotalPackages);
                }
                else if (packageMode == PackageMode.Local)
                {
                    _props.GetLocalPackageCount.Invoke(setTotalPackages);
                }
            }, new object[]{packageMode});
            Reacc.UseEffect(() =>
            {
                if (packageMode == PackageMode.Osu)
                {
                    _props.DoOsuLocalSearch.Invoke(maps =>
                    {
                        setOsuMaps.Invoke(maps);
                        setOsuError.Invoke("");
                    }, setOsuError);
                }
            }, new object[]{packageMode});

            // On local file update, reload
            Reacc.UseEffect(() =>
            {
                if (_props.PackageGrabber.LocalOutOfSync)
                {
                    // We're out of sync!
                    // Reload locally.
                    _props.GetLocalPackageCount.Invoke(setTotalPackages);
                    _props.DoLocalSearch.Invoke(searchQuery, SetPackageInfoLoaded);
                }
            }, new object[]{_props.PackageGrabber.LocalOutOfSync});

            // UI

            // Toggle button
            int buttonSize = open ? 64 : 128;
            if (GUI.Button(new Rect(Screen.width - buttonSize, Screen.height - buttonSize, buttonSize, buttonSize), "Custom"))
            {
                setOpen.Invoke(!open);
            }

            // Old beatmap conversions
            if (oldMapsDetected)
            {
                float left = Screen.width - buttonSize * 2 + 8;
                GUI.Label(new Rect(left, Screen.height - buttonSize - 18, 1000, 18), "OLD Beatmaps Detected!" );
                if (GUI.Button(
                    new Rect(left, Screen.height - buttonSize, buttonSize, buttonSize),
                    "CONVERT\nOLD"))
                {
                    setOldMapConversionOpen(!oldMapConversionOpen);
                }
            }

            if (oldMapConversionOpen)
            {
                OldModConversionUI.Render(
                    (forceMove, onLog) =>
                    {
                        _props.DoConvertOldBeatmaps(forceMove, onLog);
                        setOldMapsDetected.Invoke(false);
                    }, () => setOldMapConversionOpen.Invoke(false));
            }

            if (open && !oldMapConversionOpen)
            {
                int windowPad = 32;
                Rect centerRect = new Rect(windowPad, windowPad, Screen.width - windowPad * 2,
                    Screen.height - windowPad * 2);
                GUILayout.Window(Reacc.GetUniqueId(), centerRect, _ =>
                {
                    PackageModePicker.Render(packageMode, setPackageMode);
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

                    switch (packageMode)
                    {
                        case PackageMode.Local:
                        case PackageMode.Online:
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
                                    setSearchQuery.Invoke(new SearchQuery(searchQuery.TextQuery, searchQuery.Ascending,
                                        searchQuery.SortType, start, end));
                                });
                            // Package Preview
                            if (packageSelected)
                            {
                                CustomPackageInfo currentPackage = packageInfos[selectedPackage];
                                PackagePreview.Render(
                                    currentPackage,
                                    _props.PackageGrabber.GetDownloadStatus(currentPackage.DatabaseId),
                                    (difficultyRequested, onGrab) =>
                                        _props.DoLeaderboardSearch.Invoke(currentPackage.DatabaseId, difficultyRequested,
                                            onGrab),
                                    () => _props.OnDownloadRequest.Invoke(currentPackage.DatabaseId),
                                    difficultyToPlay =>
                                        _props.OnPlayRequest.Invoke(currentPackage.DatabaseId, difficultyToPlay),
                                    GUILayout.Width(Screen.width / 2 - windowPad * 2)
                                );
                            }
                            break;
                        case PackageMode.Osu:
                            OSUPackagePicker.Render(osuMaps, _props.OnEditOsuMap, osuError);
                            break;
                    }

                    GUILayout.EndHorizontal();
                }, "Beatmap Picker");
            }
        }

        private int CalculatePageSize()
        {
            // TODO: Depending on package list size?
            return 50;
        }
    }
}
