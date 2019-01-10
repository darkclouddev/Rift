using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rift.Data.Models
{
    public class RiftTimestamp
    {
        [ForeignKey(nameof(User)), Required, Column(Order = 0)]
        public ulong UserId { get; set; }
        public RiftUser User { get; set; }

        public ulong LastStoreTimestamp { get; set; } = 0ul;
        public ulong LastAttackTimestamp { get; set; } = 0ul;
        public ulong LastBeingAttackedTimestamp { get; set; } = 0ul;
        public ulong CreatedAtTimestamp { get; set; } = 0ul;
        public ulong LastDailyChestTimestamp { get; set; } = 0ul;
        public ulong LastBragTimestamp { get; set; } = 0ul;
        public ulong LastGiftTimestamp { get; set; } = 0ul;
        public ulong DoubleExpTimestamp { get; set; } = 0ul;
        public ulong BotRespectTimestamp { get; set; } = 0ul;
        public ulong LastLolAccountUpdateTimestamp { get; set; } = 0ul;
    }
}
