namespace Rift.Data.Models
{
    public class RiftScheduledEvent
    {
        public int Id { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public string EventName { get; set; }
    }
}
