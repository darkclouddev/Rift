using System;

namespace Rift.Data.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RiftScheduledEvent
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int EventId { get; set; }
    }
}
