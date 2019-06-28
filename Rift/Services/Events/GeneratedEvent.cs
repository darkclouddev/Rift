using System;

namespace Rift.Services.Events
{
    public class GeneratedEvent
    {
        public int DayNumber { get; set; }
        public DateTime Time { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{Time:dd.MM.yyyy HH:mm:ss} ({Time.DayOfWeek.ToString()}) | {Name}";
        }
    }
}
