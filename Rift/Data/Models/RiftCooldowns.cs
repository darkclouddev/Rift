﻿using System;

using Rift.Configuration;

namespace Rift.Data.Models
{
    public class RiftCooldowns
    {
        public ulong UserId { get; set; }
        
        public DateTime LastItemStoreTime { get; set; }

        public TimeSpan ItemStoreTimeSpan
        {
            get
            {
                var dt = DateTime.UtcNow;
                var nextTime = LastItemStoreTime + Settings.Economy.ItemStoreCooldown;
                return nextTime > dt
                    ? nextTime - dt
                    : TimeSpan.Zero;
            }
        }

        public DateTime LastRoleStoreTime { get; set; }

        public TimeSpan RoleStoreTimeSpan
        {
            get
            {
                var dt = DateTime.UtcNow;
                var nextTime = LastRoleStoreTime + Settings.Economy.RoleStoreCooldown;
                return nextTime > dt
                    ? nextTime - dt
                    : TimeSpan.Zero;
            }
        }

        public DateTime LastBackgroundStoreTime { get; set; }

        public TimeSpan BackgroundStoreTimeSpan
        {
            get
            {
                var dt = DateTime.UtcNow;
                var nextTime = LastBackgroundStoreTime + Settings.Economy.BackgroundStoreCooldown;
                return nextTime > dt
                    ? nextTime - dt
                    : TimeSpan.Zero;
            }
        }

        public DateTime LastBragTime { get; set; }

        public TimeSpan BragTimeSpan
        {
            get
            {
                var dt = DateTime.UtcNow;
                var nextTime = LastBragTime + Settings.Economy.BragCooldown;
                return nextTime > dt
                    ? nextTime - dt
                    : TimeSpan.Zero;
            }
        }

        public DateTime LastGiftTime { get; set; }

        public TimeSpan GiftTimeSpan
        {
            get
            {
                var dt = DateTime.UtcNow;
                var nextTime = LastGiftTime + Settings.Economy.GiftCooldown;
                return nextTime > dt
                    ? nextTime - dt
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

        public DateTime LastCommunityVoteTime { get; set; }

        public TimeSpan LastCommunityVoteTimeSpan
        {
            get
            {
                var dt = DateTime.UtcNow;
                var nextTime = LastCommunityVoteTime + Settings.Economy.VoteCommunityCooldown;
                return nextTime > dt
                    ? nextTime - dt
                    : TimeSpan.Zero;
            }
        }
        
        public DateTime LastStreamerVoteTime { get; set; }

        public TimeSpan StreamerVoteTimeSpan
        {
            get
            {
                var dt = DateTime.UtcNow;
                var nextTime = LastStreamerVoteTime + Settings.Economy.VoteStreamerCooldown;
                return nextTime > dt
                    ? nextTime - dt
                    : TimeSpan.Zero;
            }
        }
        
        public DateTime LastTeamVoteTime { get; set; }

        public TimeSpan TeamVoteTimeSpan
        {
            get
            {
                var dt = DateTime.UtcNow;
                var nextTime = LastTeamVoteTime + Settings.Economy.VoteTeamCooldown;
                return nextTime > dt
                    ? nextTime - dt
                    : TimeSpan.Zero;
            }
        } 
        
        public DateTime LastDailyRewardTime { get; set; }

        public TimeSpan DailyRewardTimeSpan
        {
            get
            {
                var dt = DateTime.UtcNow;
                var nextTime = LastDailyRewardTime + TimeSpan.FromHours(22);
                return nextTime > dt
                    ? nextTime - dt
                    : TimeSpan.Zero;
            }
        } 

        public RiftUser User { get; set; }
    }
}
