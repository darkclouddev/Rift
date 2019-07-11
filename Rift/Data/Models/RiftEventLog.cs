using System;

namespace Rift.Data.Models
{
    public class RiftEventLog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool HadSpecialReward => SpecialWinnerId != 0ul;
        public ulong SpecialWinnerId { get; set; }
        public uint ParticipantsAmount { get; set; }
        public string Reward { get; set; }
        public ulong StartedBy { get; set; }
        public DateTime StartedAt { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime FinishedAt { get; set; }
    }
}
