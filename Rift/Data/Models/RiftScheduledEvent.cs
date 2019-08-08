using System;

namespace Rift.Data.Models
{
    public class RiftScheduledEvent
    {
        public int Id { get; set; }
        public DateTime StartAt { get; set; }
        public int EventType { get; set; }
    }
}
