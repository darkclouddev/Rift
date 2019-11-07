using System;

namespace Rift.Data.Models
{
    public class RiftActiveEvent
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public Guid MessageId { get; set; }
        public ulong ChannelMessageId { get; set; }
        public ulong StartedBy { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime DueTime { get; set; }
    }
}
