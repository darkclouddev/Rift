using System;

namespace Rift.Data.Models
{
    public class RiftGiveaway
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public uint WinnersAmount { get; set; }
        public Guid MessageId { get; set; }
        public int RewardId { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime CreatedAt { get; set; }
        public ulong CreatedBy { get; set; }
    }
}
