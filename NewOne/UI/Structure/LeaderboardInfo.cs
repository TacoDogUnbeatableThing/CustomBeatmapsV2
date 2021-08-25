using System.Collections.Generic;

namespace CustomBeatmaps.UI.Structure
{
    public struct LeaderboardEntry
    {
        // TODO: Later, Fill with leaderboard info
    }
    public class LeaderboardInfo
    {
        private readonly Dictionary<string, List<LeaderboardEntry>> _leaderboardRankings = new Dictionary<string, List<LeaderboardEntry>>();

        public List<LeaderboardEntry> GetRanks(string difficulty)
        {
            if (_leaderboardRankings.ContainsKey(difficulty))
            {
                return _leaderboardRankings[difficulty];
            }
            return new List<LeaderboardEntry>(); // Empty
        }

        public static readonly LeaderboardInfo Empty = new LeaderboardInfo();
    }
}
