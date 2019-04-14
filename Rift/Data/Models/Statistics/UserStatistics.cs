namespace Rift.Data.Models.Statistics
{
    public class UserStatistics
    {
        public ulong UserId;
        public uint Level;

        public uint CoinsEarnedTotal;
        public uint TokensEarnedTotal;
        public uint ChestsEarnedTotal;
        public uint SphereEarnedTotal;
        public uint CapsuleEarnedTotal;

        public uint ChestsOpenedTotal;
        public uint SphereOpenedTotal;
        public uint CapsuleOpenedTotal;

        public uint AttacksDone;
        public uint AttacksReceived;

        public uint CoinsSpentTotal;
        public uint TokensSpentTotal;
        public uint GiftsSent;
        public uint GiftsReceived;
        public uint MessagesSentTotal;
        public uint BragTotal;
        public uint PurchasedItemsTotal;

        public decimal Donate;
    }
}
