using System;

namespace Rift.Data.Models
{
    public class RiftGiveawayLog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ulong[] Winners // not mapped
        {
            get
            {
                if (string.IsNullOrEmpty(WinnersString))
                    return new ulong[]{};
                
                return Array.ConvertAll(WinnersString.Split(';'), ulong.Parse);
            }
            set
            {
                if (value is null || value.Length == 0)
                {
                    WinnersString = string.Empty;
                    return;
                }
                
                WinnersString = string.Join(';', value);
            }
        }

        public string WinnersString { get; set; }

        public ulong[] Participants // not mapped
        {
            get
            {
                if (string.IsNullOrEmpty(ParticipantsString))
                    return new ulong[]{};
                
                return Array.ConvertAll(ParticipantsString.Split(';'), ulong.Parse);
            }
            set
            {
                if (value is null || value.Length == 0)
                {
                    ParticipantsString = string.Empty;
                    return;
                }
                
                ParticipantsString = string.Join(';', value);
            }
        }

        public string ParticipantsString { get; set; }
        public string Reward { get; set; }
        public ulong StartedBy { get; set; }
        public DateTime StartedAt { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime FinishedAt { get; set; }
    }
}
