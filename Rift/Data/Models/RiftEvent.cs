using System;

namespace Rift.Data.Models
{
    public class RiftEvent
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public Guid MessageId { get; set; }
        public int SharedRewardId { get; set; }
        public int SpecialRewardId { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime CreatedAt { get; set; }
        public ulong CreatedBy { get; set; }
        
        public bool HasSpecialReward => SpecialRewardId != 0;
    }
}
