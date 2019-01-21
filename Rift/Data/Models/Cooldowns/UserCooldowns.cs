using System;

namespace Rift.Data.Models.Cooldowns
{
    public class UserCooldowns
    {
        public ulong UserId { get; set; }
        public DateTime LastStoreTime { get; set; }
        public TimeSpan LastStoreTimeSpan { get; set; }
        public DateTime LastAttackTime { get; set; }
        public TimeSpan LastAttackTimeSpan { get; set; }
        public DateTime LastBeingAttackedTime { get; set; }
        public TimeSpan LastBeingAttackedTimeSpan { get; set; }
        public DateTime LastDailyChestTime { get; set; }
        public TimeSpan LastDailyChestTimeSpan { get; set; }
        public DateTime LastBragTime { get; set; }
        public TimeSpan LastBragTimeSpan { get; set; }
        public DateTime LastGiftTime { get; set; }
        public TimeSpan LastGiftTimeSpan { get; set; }
        public DateTime DoubleExpTime { get; set; }
        public TimeSpan DoubleExpTimeSpan { get; set; }
        public DateTime BotRespectTime { get; set; }
        public TimeSpan BotRespectTimeSpan { get; set; }
        public DateTime LastLolAccountUpdateTime { get; set; }
        public TimeSpan LastLolAccountUpdateTimeSpan { get; set; }

        public UserCooldowns(RiftCooldowns cooldowns)
        {
            UserId = cooldowns.UserId;
            
            var dt = DateTime.UtcNow;
            
            LastStoreTime = cooldowns.LastStoreTime;
            LastStoreTimeSpan = dt > LastStoreTime
                                    ? dt - LastStoreTime
                                    : TimeSpan.Zero;
            
            LastAttackTime = cooldowns.LastAttackTime;
            LastAttackTimeSpan = dt > LastAttackTime
                                    ? dt - LastAttackTime
                                    : TimeSpan.Zero;
            
            LastBeingAttackedTime = cooldowns.LastBeingAttackedTime;
            LastBeingAttackedTimeSpan = dt > LastBeingAttackedTime
                                            ? dt - LastBeingAttackedTime
                                            : TimeSpan.Zero;
            
            LastDailyChestTime = cooldowns.LastDailyChestTime;
            LastDailyChestTimeSpan = dt > LastDailyChestTime
                                        ? dt - LastDailyChestTime
                                        : TimeSpan.Zero;
            
            LastBragTime = cooldowns.LastBragTime;
            LastBragTimeSpan = dt > LastBragTime
                                    ? dt - LastBragTime
                                    : TimeSpan.Zero;
            
            LastGiftTime = cooldowns.LastGiftTime;
            LastGiftTimeSpan = dt > LastGiftTime
                                    ? dt - LastGiftTime
                                    : TimeSpan.Zero;
            
            DoubleExpTime = cooldowns.DoubleExpTime;
            DoubleExpTimeSpan = dt > DoubleExpTime
                                    ? dt - DoubleExpTime
                                    : TimeSpan.Zero;
            
            BotRespectTime = cooldowns.BotRespectTime;
            BotRespectTimeSpan = dt > BotRespectTime
                                    ? dt - BotRespectTime
                                    : TimeSpan.Zero;
            
            LastLolAccountUpdateTime = cooldowns.LastLolAccountUpdateTime;
            LastLolAccountUpdateTimeSpan = dt > LastLolAccountUpdateTime
                                                ? dt - LastLolAccountUpdateTime
                                                : TimeSpan.Zero;
        }
    }
}
