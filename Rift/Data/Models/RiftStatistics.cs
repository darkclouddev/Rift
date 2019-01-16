namespace Rift.Data.Models
{
    public class RiftStatistics
    {
        public ulong UserId { get; set; }
        public ulong CoinsEarnedTotal { get; set; } = 0ul;
        public ulong TokensEarnedTotal { get; set; } = 0ul;
        public ulong ChestsEarnedTotal { get; set; } = 0ul;
        public ulong SphereEarnedTotal { get; set; } = 0ul;
        public ulong CapsuleEarnedTotal { get; set; } = 0ul;
        public ulong ChestsOpenedTotal { get; set; } = 0ul;
        public ulong SphereOpenedTotal { get; set; } = 0ul;
        public ulong CapsuleOpenedTotal { get; set; } = 0ul;
        public ulong AttacksDone { get; set; } = 0ul;
        public ulong AttacksReceived { get; set; } = 0ul;
        public ulong CoinsSpentTotal { get; set; } = 0ul;
        public ulong TokensSpentTotal { get; set; } = 0ul;
        public ulong GiftsSent { get; set; } = 0ul;
        public ulong GiftsReceived { get; set; } = 0ul;
        public ulong MessagesSentTotal { get; set; } = 0ul;
        public ulong BragTotal { get; set; } = 0ul;
        public ulong PurchasedItemsTotal { get; set; } = 0ul;

        public RiftUser User { get; set; }
    }
}
