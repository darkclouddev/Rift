using System;

namespace Rift.Configuration
{
    public class Economy
    {
        public TimeSpan MessageCooldown { get; set; } = TimeSpan.FromSeconds(15);
        public TimeSpan GiftCooldown { get; set; } = TimeSpan.FromHours(1);
        public TimeSpan ItemStoreCooldown { get; set; } = TimeSpan.FromMinutes(60);
        public TimeSpan RoleStoreCooldown { get; set; } = TimeSpan.FromHours(24);
        public TimeSpan BackgroundStoreCooldown { get; set; } = TimeSpan.FromMinutes(60);
        public TimeSpan BragCooldown { get; set; } = TimeSpan.FromHours(6);
        public TimeSpan VoteCommunityCooldown { get; set; } = TimeSpan.FromDays(14);
        public TimeSpan VoteTeamCooldown { get; set; } = TimeSpan.FromDays(14);
        public TimeSpan VoteStreamerCooldown { get; set; } = TimeSpan.FromDays(14);
        public int BragWinCoinsMin { get; set; } = 0;
        public int BragWinCoinsMax { get; set; } = 0;
        public int BragLossCoinsMin { get; set; } = 0;
        public int BragLossCoinsMax { get; set; } = 0;
        public TimeSpan PendingUserLifeTime { get; set; } = TimeSpan.FromHours(4);
        public TimeSpan LolAccountUpdateCooldown { get; set; } = TimeSpan.FromHours(6);
        public uint GiftPrice { get; set; } = 0u;
        public TimeSpan ToxicityWaitTime { get; set; } = TimeSpan.FromDays(30);
        public uint ToxicityWeeklyDropRate { get; set; } = 1u;
    }
}
