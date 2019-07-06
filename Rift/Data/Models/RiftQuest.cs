using System;

namespace Rift.Data.Models
{
    public class RiftQuest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StageId { get; set; }
        public int? RewardId { get; set; }
        public int Order { get; set; }
        public bool? ApprovedLolAccount { get; set; }
        public uint? BragsDone { get; set; }
        public uint? MessagesSent { get; set; }
        public uint? BoughtChests { get; set; }
        public uint? OpenedChests { get; set; }
        public uint? UsualMonstersKilled { get; set; } // TODO: after monsters
        public uint? RareMonstersKilled { get; set; } // TODO: after monsters
        public uint? EpicMonstersKilled { get; set; } // TODO: after monsters
        public uint TotalMonstersKilled =>
            (UsualMonstersKilled ?? 0u) + (RareMonstersKilled ?? 0u) + (EpicMonstersKilled ?? 0u);
        public uint? GiftsSent { get; set; }
        public uint? GiftsReceived { get; set; }
        public uint? GiftsReceivedFromUltraGay { get; set; }
        public uint? LevelReached { get; set; }
        public uint? GiveawaysParticipated { get; set; } // TODO: after giveaways
        public uint? CoinsReceived { get; set; }
        public uint? CoinsSpent { get; set; }
        public TimeSpan? VoiceUptimeEarned { get; set; }
        public bool? GiftedBotKeeper { get; set; }
        public bool? GiftedModerator { get; set; }
        public bool? GiftedStreamer { get; set; }
        public uint? ActivatedBotRespects { get; set; }
        public bool? OpenedSphere { get; set; }
        public uint? RolesPurchased { get; set; }
    }
}
