using System;

using Rift.Configuration;

namespace Rift.Data.Models.Cooldowns
{
    public class UserCooldowns
    {
        public ulong UserId { get; }
        public DateTime LastStoreTime { get; }
        public TimeSpan StoreTimeSpan { get; }
        public DateTime LastAttackTime { get; }
        public TimeSpan AttackTimeSpan { get; }
        public DateTime LastBeingAttackedTime { get; }
        public TimeSpan BeingAttackedTimeSpan { get; }
        public DateTime LastDailyChestTime { get; }
        public TimeSpan DailyChestTimeSpan { get; }
        public DateTime LastBragTime { get; }
        public TimeSpan BragTimeSpan { get; }
        public DateTime LastGiftTime { get; }
        public TimeSpan GiftTimeSpan { get; }
        public DateTime DoubleExpTime { get; }
        public TimeSpan DoubleExpTimeSpan { get; }
        public DateTime BotRespectTime { get; }
        public TimeSpan BotRespectTimeSpan { get; }
        public DateTime LastLolAccountUpdateTime { get; }
        public TimeSpan LolAccountUpdateTimeSpan { get; }

        public UserCooldowns(RiftCooldowns cooldowns)
        {
            UserId = cooldowns.UserId;
            
            var dt = DateTime.UtcNow;
            
            LastStoreTime = cooldowns.LastStoreTime;
            var nextStoreTime = LastStoreTime + Settings.Economy.StoreCooldown;
            StoreTimeSpan = nextStoreTime > dt ? nextStoreTime - dt : TimeSpan.Zero;
            
            LastAttackTime = cooldowns.LastAttackTime;
            var nextAttackTime = LastAttackTime + Settings.Economy.AttackPerUserCooldown;
            AttackTimeSpan = nextAttackTime > dt ? nextAttackTime - dt : TimeSpan.Zero;
            
            LastBeingAttackedTime = cooldowns.LastBeingAttackedTime;
            var nextAttackedTime = LastBeingAttackedTime + Settings.Economy.AttackSameUserCooldown;
            BeingAttackedTimeSpan = nextAttackedTime > dt ? nextAttackedTime - dt : TimeSpan.Zero;
            
            LastDailyChestTime = cooldowns.LastDailyChestTime;
            var nextDailyTime = LastDailyChestTime + TimeSpan.FromDays(1);
            DailyChestTimeSpan = nextDailyTime > dt ? nextDailyTime - dt : TimeSpan.Zero;
            
            LastBragTime = cooldowns.LastBragTime;
            var nextBragTime = LastBragTime + Settings.Economy.BragCooldown;
            BragTimeSpan = nextBragTime > dt ? nextBragTime - dt : TimeSpan.Zero;
            
            LastGiftTime = cooldowns.LastGiftTime;
            var nextGiftTime = LastGiftTime + Settings.Economy.GiftCooldown;
            GiftTimeSpan = nextGiftTime > dt ? nextGiftTime - dt : TimeSpan.Zero;
            
            DoubleExpTime = cooldowns.DoubleExpTime;
            DoubleExpTimeSpan = DoubleExpTime > dt ? DoubleExpTime - dt : TimeSpan.Zero;
            
            BotRespectTime = cooldowns.BotRespectTime;
            BotRespectTimeSpan = BotRespectTime > dt ? BotRespectTime - dt : TimeSpan.Zero;
            
            LastLolAccountUpdateTime = cooldowns.LastLolAccountUpdateTime;
            LolAccountUpdateTimeSpan = dt > LastLolAccountUpdateTime ? dt - LastLolAccountUpdateTime : TimeSpan.Zero;
        }
    }
}
