using System;

namespace Rift.Data.Models
{
    public class RiftGiveawayLog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ulong[] Winners // not mapped
        {
            get => Array.ConvertAll(WinnersString.Split(';'), ulong.Parse);
            set => WinnersString = string.Join(';', value);
        }
        public string WinnersString { get; set; }

        public ulong[] Participants
        {
            get => Array.ConvertAll(ParticipantsString.Split(';'), ulong.Parse);
            set => ParticipantsString = string.Join(';', value);
        }
        public string ParticipantsString { get; set; }
        public string Reward { get; set; }
        public ulong StartedBy { get; set; }
        public DateTime StartedAt { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime FinishedAt { get; set; }
    }
}
