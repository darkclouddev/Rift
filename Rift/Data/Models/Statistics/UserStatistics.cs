namespace Rift.Data.Models.Statistics
{
    public class UserStatistics
    {
        public ulong UserId;
        public uint Level;
        public uint AchievementsCount;

        public ulong CoinsEarnedTotal;
        public ulong TokensEarnedTotal;
        public ulong ChestsEarnedTotal;
        public ulong SphereEarnedTotal;
        public ulong CapsuleEarnedTotal;

        public ulong ChestsOpenedTotal;
        public ulong SphereOpenedTotal;
        public ulong CapsuleOpenedTotal;

        public ulong AttacksDone;
        public ulong AttacksReceived;

        public ulong CoinsSpentTotal;
        public ulong TokensSpentTotal;
        public ulong GiftsSent;
        public ulong GiftsReceived;
        public ulong MessagesSentTotal;
        public ulong BragTotal;
        public ulong PurchasedItemsTotal;

        public decimal Donate;
    }
}
