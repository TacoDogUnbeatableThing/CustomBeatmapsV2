using System;
using CustomBeatmaps.UI.Structure;
using UnityEngine;

namespace CustomBeatmaps.UI.ReactEsque
{
    public static class SearchBar
    {
        public static void Render(SearchQuery search, Action<SearchQuery> setSearch)
        {
            GUILayout.BeginHorizontal();
                string newText = GUILayout.TextField(search.TextQuery, GUILayout.ExpandWidth(true));
                // TODO: Some sort of ugly "don't modify data" paradigm here that C# isn't designed to handle...
                if (newText != search.TextQuery)
                {
                    setSearch(new SearchQuery(newText, search.Ascending, search.SortType, search.StartPackage,
                        search.EndPackage));
                }
                Dropdown.RenderEnum(search.SortType, newSortType =>
                {
                    setSearch(new SearchQuery(search.TextQuery, search.Ascending, newSortType, search.StartPackage,
                        search.EndPackage));
                }, GUILayout.Width(128));
            GUILayout.EndHorizontal();
        }
    }
}
