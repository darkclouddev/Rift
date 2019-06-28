using System;

namespace Rift.Data.Models
{
    public class RiftStatistics
    {
        public ulong UserId { get; set; }
        public uint CoinsEarned { get; set; }
        public uint CoinsSpent { get; set; }
        public uint TokensEarned { get; set; }
        public uint TokensSpent { get; set; }
        public uint EssenceEarned { get; set; }
        public uint EssenceSpent { get; set; }
        public uint ChestsEarned { get; set; }
        public uint ChestsOpened { get; set; }
        public uint SpheresEarned { get; set; }
        public uint SpheresOpened { get; set; }
        public uint CapsulesEarned { get; set; }
        public uint CapsulesOpened { get; set; }
        public uint TicketsEarned { get; set; }
        public uint TicketsSpent { get; set; }
        public uint DoubleExpsEarned { get; set; }
        public uint DoubleExpsActivated { get; set; }
        public uint BotRespectsEarned { get; set; }
        public uint BotRespectsActivated { get; set; }
        public uint RewindsEarned { get; set; }
        public uint RewindsActivated { get; set; }
        public uint GiftsSent { get; set; }
        public uint GiftsReceived { get; set; }
        public uint MessagesSent { get; set; }
        public uint BragsDone { get; set; }
        public uint PurchasedItems { get; set; }
        public TimeSpan VoiceUptime { get; set; }

        public RiftUser User { get; set; }
    }
}
