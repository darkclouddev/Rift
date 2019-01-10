namespace Rift.Services.Riot
{
    public class RankChangedEventArgs
    {
        public readonly ulong UserId;
        public readonly LeagueRank OldRank;
        public readonly LeagueRank NewRank;
        public readonly bool IsUp;

        public RankChangedEventArgs(ulong userId, LeagueRank oldRank, LeagueRank newRank, bool isUp)
        {
            UserId = userId;
            OldRank = oldRank;
            NewRank = newRank;
            IsUp = isUp;
        }
    }
}
