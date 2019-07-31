using System;

namespace Rift.Data.Models
{
    public class RiftQuestProgress
    {
        public ulong UserId { get; set; }
        public int QuestId { get; set; }
        public bool IsCompleted { get; set; }
        public bool? ApprovedLolAccount { get; set; }
        public uint? BragsDone { get; set; }
        public uint? MessagesSent { get; set; }
        public uint? BoughtChests { get; set; }
        public uint? OpenedChests { get; set; }
        public uint? NormalMonstersKilled { get; set; }
        public uint? RareMonstersKilled { get; set; }
        public uint? EpicMonstersKilled { get; set; }
        public uint? GiftsSent { get; set; }
        public uint? GiftsReceived { get; set; }
        public uint? GiftsReceivedFromUltraGay { get; set; }
        public uint? LevelReached { get; set; }
        public uint? GiveawaysParticipated { get; set; }
        public uint? CoinsReceived { get; set; }
        public uint? CoinsSpent { get; set; }
        public TimeSpan? VoiceUptimeEarned { get; set; }
        public bool? GiftedDeveloper { get; set; }
        public bool? GiftedModerator { get; set; }
        public bool? GiftedStreamer { get; set; }
        public uint? ActivatedBotRespects { get; set; }
        public bool? OpenedSphere { get; set; }
        public uint? RolesPurchased { get; set; }
    }
}
