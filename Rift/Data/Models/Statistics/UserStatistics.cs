using System;

namespace Rift.Data.Models.Statistics
{
    public class UserStatistics
    {
        public UInt64 UserId;
        public UInt32 Level;

        public UInt32 CoinsEarnedTotal;
        public UInt32 TokensEarnedTotal;
        public UInt32 ChestsEarnedTotal;
        public UInt32 SphereEarnedTotal;
        public UInt32 CapsuleEarnedTotal;

        public UInt32 ChestsOpenedTotal;
        public UInt32 SphereOpenedTotal;
        public UInt32 CapsuleOpenedTotal;

        public UInt32 AttacksDone;
        public UInt32 AttacksReceived;

        public UInt32 CoinsSpentTotal;
        public UInt32 TokensSpentTotal;
        public UInt32 GiftsSent;
        public UInt32 GiftsReceived;
        public UInt32 MessagesSentTotal;
        public UInt32 BragTotal;
        public UInt32 PurchasedItemsTotal;

        public Decimal Donate;
    }
}
