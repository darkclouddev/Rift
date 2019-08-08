using System;

namespace Rift.Data.Models
{
    public class RiftSystemTimer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TimeSpan Interval { get; set; }
        public DateTime LastInvoked { get; set; }
    }
}
