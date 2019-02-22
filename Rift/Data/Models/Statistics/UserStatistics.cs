using System;

namespace Rift.Data.Models.Statistics
{
    public class UserStatistics
    {
        public UInt64 UserId;
        public UInt32 Level;

        public UInt64 CoinsEarnedTotal;
        public UInt64 TokensEarnedTotal;
        public UInt64 ChestsEarnedTotal;
        public UInt64 SphereEarnedTotal;
        public UInt64 CapsuleEarnedTotal;

        public UInt64 ChestsOpenedTotal;
        public UInt64 SphereOpenedTotal;
        public UInt64 CapsuleOpenedTotal;

        public UInt64 AttacksDone;
        public UInt64 AttacksReceived;

        public UInt64 CoinsSpentTotal;
        public UInt64 TokensSpentTotal;
        public UInt64 GiftsSent;
        public UInt64 GiftsReceived;
        public UInt64 MessagesSentTotal;
        public UInt64 BragTotal;
        public UInt64 PurchasedItemsTotal;

        public Decimal Donate;
    }
}
