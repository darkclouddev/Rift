using System;

namespace Rift.Data.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ScheduledEvent
    {
        public UInt32 Id { get; set; }
        public Int32 DayId { get; set; }
        public Int32 EventId { get; set; }
        public Int32 Hour { get; set; }
        public Int32 Minute { get; set; }
    }
}
