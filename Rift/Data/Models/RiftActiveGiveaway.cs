using System;

namespace Rift.Data.Models
{
    public class RiftActiveGiveaway
    {
        public int Id { get; set; }
        public string GiveawayName { get; set; }
        public int StoredMessageId { get; set; }
        public ulong ChannelMessageId { get; set; }
        public ulong StartedBy { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime DueTime { get; set; }
    }
}
