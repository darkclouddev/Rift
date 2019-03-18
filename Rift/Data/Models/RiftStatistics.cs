using System;

namespace Rift.Data.Models
{
    public class RiftStatistics
    {
        public UInt64 UserId { get; set; }
        public UInt32 CoinsEarnedTotal { get; set; }
        public UInt32 TokensEarnedTotal { get; set; }
        public UInt32 ChestsEarnedTotal { get; set; }
        public UInt32 SphereEarnedTotal { get; set; }
        public UInt32 CapsuleEarnedTotal { get; set; }
        public UInt32 ChestsOpenedTotal { get; set; }
        public UInt32 SphereOpenedTotal { get; set; }
        public UInt32 CapsuleOpenedTotal { get; set; }
        public UInt32 AttacksDone { get; set; }
        public UInt32 AttacksReceived { get; set; }
        public UInt32 CoinsSpentTotal { get; set; }
        public UInt32 TokensSpentTotal { get; set; }
        public UInt32 GiftsSent { get; set; }
        public UInt32 GiftsReceived { get; set; }
        public UInt32 MessagesSentTotal { get; set; }
        public UInt32 BragTotal { get; set; }
        public UInt32 PurchasedItemsTotal { get; set; }

        public RiftUser User { get; set; }
    }
}
