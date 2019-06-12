using System;

using Rift.Configuration;

namespace Rift.Data.Models
{
    public class RiftCooldowns
    {
        public ulong UserId { get; set; }
        public DateTime LastStoreTime { get; set; }
        public TimeSpan StoreTimeSpan
        {
            get
            {
                var dt = DateTime.UtcNow;
                var nextStoreTime = LastStoreTime + Settings.Economy.StoreCooldown;
                return nextStoreTime > dt
                    ? nextStoreTime - dt
                    : TimeSpan.Zero;
            }
        }
        public DateTime LastDailyChestTime { get; set; }
        public TimeSpan DailyChestTimeSpan
        {
            get
            {
                var dt = DateTime.UtcNow;
                var nextDailyTime = LastDailyChestTime + TimeSpan.FromHours(22);
                return nextDailyTime > dt
                    ? nextDailyTime - dt
                    : TimeSpan.Zero;
            }
        }
        public DateTime LastBragTime { get; set; }
        public TimeSpan BragTimeSpan
        {
            get
            {
                var dt = DateTime.UtcNow;
                var nextBragTime = LastBragTime + Settings.Economy.BragCooldown;
                return nextBragTime > dt
                    ? nextBragTime - dt
                    : TimeSpan.Zero;
            }
        }
        public DateTime LastGiftTime { get; set; }
        public TimeSpan GiftTimeSpan
        {
            get
            {
                var dt = DateTime.UtcNow;
                var nextGiftTime = LastGiftTime + Settings.Economy.GiftCooldown;
                return nextGiftTime > dt
                    ? nextGiftTime - dt
                    : TimeSpan.Zero;
            }
        }
        public DateTime DoubleExpTime { get; set; }
        public TimeSpan DoubleExpTimeSpan
        {
            get
            {
                var dt = DateTime.UtcNow;
                return DoubleExpTime > dt
                    ? DoubleExpTime - dt
                    : TimeSpan.Zero;
            }
        }
        public DateTime BotRespectTime { get; set; }
        public TimeSpan BotRespectTimeSpan
        {
            get
            {
                var dt = DateTime.UtcNow;
                return BotRespectTime > dt
                    ? BotRespectTime - dt
                    : TimeSpan.Zero;
            }
        }
        public DateTime LastLolAccountUpdateTime { get; set; }
        public TimeSpan LolAccountUpdateTimeSpan
        {
            get
            {
                var dt = DateTime.UtcNow;
                return dt > LastLolAccountUpdateTime
                    ? dt - LastLolAccountUpdateTime
                    : TimeSpan.Zero;
            }
        }

        public RiftUser User { get; set; }
    }
}
