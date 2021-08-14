namespace CustomBeatmaps.UI.Structure
{
    public struct SearchQuery
    {
        public readonly string TextQuery;
        public readonly bool Ascending;
        public readonly SortType SortType;
        public readonly int StartPackage;
        public readonly int EndPackage;

        public SearchQuery(string textQuery, bool ascending, SortType sortType, int startPackage, int endPackage)
        {
            TextQuery = textQuery;
            Ascending = ascending;
            SortType = sortType;
            StartPackage = startPackage;
            EndPackage = endPackage;
        }
    }
}
