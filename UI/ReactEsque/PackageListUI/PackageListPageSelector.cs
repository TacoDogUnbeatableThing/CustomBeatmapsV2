using System;
using UnityEngine;

namespace CustomBeatmaps.UI.ReactEsque.PackageListUI
{
    public static class PackageListPageSelector
    {
        public static void Render(int pageNumber, int totalPages, Action<int> setPageNumber)
        {
            // Page selector
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label($"{pageNumber} / {totalPages}");
            if (totalPages > 1)
            {
                int bWidth = 32;
                bool useLeft = pageNumber > 1;
                bool useRight = pageNumber < totalPages;
                if (useLeft != useRight)
                {
                    // Only one button
                    bWidth *= 2;
                }
                if (useLeft && GUILayout.Button("<", GUILayout.Width(bWidth)))
                {
                    setPageNumber.Invoke(pageNumber - 1);
                }
                if (useRight && GUILayout.Button(">", GUILayout.Width(bWidth)))
                {
                    setPageNumber.Invoke(pageNumber + 1);
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}