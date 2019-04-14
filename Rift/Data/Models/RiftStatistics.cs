namespace Rift.Data.Models
{
    public class RiftStatistics
    {
        public ulong UserId { get; set; }
        public uint CoinsEarnedTotal { get; set; }
        public uint TokensEarnedTotal { get; set; }
        public uint ChestsEarnedTotal { get; set; }
        public uint SphereEarnedTotal { get; set; }
        public uint CapsuleEarnedTotal { get; set; }
        public uint ChestsOpenedTotal { get; set; }
        public uint SphereOpenedTotal { get; set; }
        public uint CapsuleOpenedTotal { get; set; }
        public uint AttacksDone { get; set; }
        public uint AttacksReceived { get; set; }
        public uint CoinsSpentTotal { get; set; }
        public uint TokensSpentTotal { get; set; }
        public uint GiftsSent { get; set; }
        public uint GiftsReceived { get; set; }
        public uint MessagesSentTotal { get; set; }
        public uint BragTotal { get; set; }
        public uint PurchasedItemsTotal { get; set; }

        public RiftUser User { get; set; }
    }
}
