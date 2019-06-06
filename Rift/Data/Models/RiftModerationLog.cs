using System;

namespace Rift.Data.Models
{
    public class RiftModerationLog
    {
        public uint Id { get; set; }
        public ulong ModeratorId { get; set; }
        public ulong TargetId { get; set; }
        public string  Action { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
