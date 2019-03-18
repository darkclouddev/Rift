using System;

namespace Rift.Data.Models
{
    public class RiftCooldowns
    {
        public UInt64 UserId { get; set; }
        public DateTime LastStoreTime { get; set; }
        public DateTime LastAttackTime { get; set; }
        public DateTime LastBeingAttackedTime { get; set; }
        public DateTime LastDailyChestTime { get; set; }
        public DateTime LastBragTime { get; set; }
        public DateTime LastGiftTime { get; set; }
        public DateTime DoubleExpTime { get; set; }
        public DateTime BotRespectTime { get; set; }
        public DateTime LastLolAccountUpdateTime { get; set; }

        public RiftUser User { get; set; }
    }
}
