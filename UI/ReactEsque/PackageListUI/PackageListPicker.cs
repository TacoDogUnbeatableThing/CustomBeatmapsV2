using System;
using System.Collections.Generic;
using CustomBeatmaps.Packages;
using CustomBeatmaps.UISystem;
using UnityEngine;

namespace CustomBeatmaps.UI.ReactEsque.PackageListUI
{
    public static class PackageListPicker
    {
        public static void Render(
            ICollection<CustomPackageInfo> packages, int selectedPackage, Action<int> setSelectedPackage, 
            int pageNumber, int totalPages, Action<int> setPageNumber)
        {
            (Vector2 scrollPos, var setScrollPos) = Reacc.UseState(Vector2.zero);

            Vector2 newScroll = GUILayout.BeginScrollView(scrollPos);
            setScrollPos.Invoke(newScroll);
            int i = 0;
            foreach (CustomPackageInfo packageInfo in packages)
            {
                bool selected = (i == selectedPackage);
                var currIndex = i;
                PackageEntry.Render(packageInfo, selected, () => setSelectedPackage.Invoke(currIndex));
                ++i;
            }
            GUILayout.EndScrollView();

            PackageListPageSelector.Render(pageNumber, totalPages, setPageNumber);
        }
    }
}