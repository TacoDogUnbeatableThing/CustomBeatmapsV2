using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomBeatmaps.UI.BeatmapPicker
{
    public static class PackageList
    {
        public static void Render(int range, int totalCount, Func<int, int, ICollection<CustomPackageStrippedInfo>> grabPackages, Action<PackageId> onSelect)
        {

            (int currentPage, var setCurrentPage) = UI.UseState(0);
            (Vector2 scrollPos, var setScrollPos) = UI.UseState(Vector2.zero);
            (int selectIndex, var setSelectIndex) = UI.UseState(-1);

            List<CustomPackageStrippedInfo> packages = new List<CustomPackageStrippedInfo>(grabPackages.Invoke(currentPage*range, (currentPage+1)*range));

            setScrollPos(GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true)));
            int index = 0;
            foreach (CustomPackageStrippedInfo package in packages)
            {
                int currentIndex = index;
                RenderPackageItem(package, index == selectIndex, () =>
                {
                    setSelectIndex(currentIndex);
                    onSelect(package.PackageId);
                });
                index++;
            }
            GUILayout.EndScrollView();

            int totalPages = (int) Math.Ceiling((float) totalCount / range);

            RenderPageMover(currentPage, totalPages, 
                () => setCurrentPage((int) Math.Clamp(currentPage - 1, 0, totalPages)), 
                () => setCurrentPage((int) Math.Clamp(currentPage + 1, 0, totalPages))
            );
        }

        private static void RenderPackageItem(CustomPackageStrippedInfo info, bool selected, Action onSelect)
        {
            if (GUILayout.Button(info.PackageName))
            {
                onSelect();
            }
        }

        private static void RenderPageMover(int currentPage, int totalPages, Action onPrev, Action onNext)
        {
            if (totalPages > 1)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                int buttonWidth = 64;
                if (currentPage > 0)
                {
                    if (GUILayout.Button(" < ", GUILayout.Width(buttonWidth)))
                    {
                        onPrev();
                    }
                }

                if (currentPage < totalPages - 1)
                {
                    if (GUILayout.Button(" > ", GUILayout.Width(buttonWidth)))
                    {
                        onNext();
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.Label($"Page {currentPage+1} / {totalPages}", GUILayout.Width(128));
                GUILayout.EndHorizontal();
            }

        }
    }
}
