using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Rift.Data;
using Rift.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace Rift.Database
{
    public class Cooldowns
    {
        static async Task<bool> EnsureExistsAsync(ulong userId)
        {
            if (!await DB.Users.EnsureExistsAsync(userId))
                return false;

            await using var context = new RiftContext();
            if (await context.Cooldowns.AnyAsync(x => x.UserId == userId))
                return true;

            try
            {
                var entry = new RiftCooldowns
                {
                    UserId = userId,
                };

                await context.Cooldowns.AddAsync(entry);
                await context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                RiftBot.Log.Error(ex, $"Failed to check {nameof(EnsureExistsAsync)} for user {userId.ToString()}.");
                return false;
            }
        }

        public async Task<RiftCooldowns> GetAsync(ulong userId)
        {
            if (!await EnsureExistsAsync(userId))
                throw new DatabaseException(nameof(GetAsync));

            await using var context = new RiftContext();
            return await context.Cooldowns
                .Where(x => x.UserId == userId)
                .FirstAsync();
        }

        public async Task<List<ulong>> GetBotRespectedUsersAsync()
        {
            await using var context = new RiftContext();
            var dt = DateTime.UtcNow;
                
            return await context.Cooldowns
                .Where(x => x.BotRespectTime > dt)
                .Select(x => x.UserId)
                .ToListAsync();;
        }

        public async Task SetLastItemStoreTimeAsync(ulong userId, DateTime time)
        {
            if (!await EnsureExistsAsync(userId))
                throw new DatabaseException(nameof(SetLastItemStoreTimeAsync));

            var cd = new RiftCooldowns
            {
                UserId = userId,
                LastItemStoreTime = time,
            };

            await using var context = new RiftContext();
            context.Attach(cd).Property(x => x.LastItemStoreTime).IsModified = true;
            await context.SaveChangesAsync();
        }

        public async Task SetLastRoleStoreTimeAsync(ulong userId, DateTime time)
        {
            if (!await EnsureExistsAsync(userId))
                throw new DatabaseException(nameof(SetLastRoleStoreTimeAsync));

            var cd = new RiftCooldowns
            {
                UserId = userId,
                LastRoleStoreTime = time,
            };

            await using var context = new RiftContext();
            context.Attach(cd).Property(x => x.LastRoleStoreTime).IsModified = true;
            await context.SaveChangesAsync();
        }

        public async Task SetLastBackgroundStoreTimeAsync(ulong userId, DateTime time)
        {
            if (!await EnsureExistsAsync(userId))
                throw new DatabaseException(nameof(SetLastBackgroundStoreTimeAsync));

            var cd = new RiftCooldowns
            {
                UserId = userId,
                LastBackgroundStoreTime = time,
            };

            await using var context = new RiftContext();
            context.Attach(cd).Property(x => x.LastBackgroundStoreTime).IsModified = true;
            await context.SaveChangesAsync();
        }

        public async Task SetDoubleExpTimeAsync(ulong userId, DateTime time)
        {
            if (!await EnsureExistsAsync(userId))
                throw new DatabaseException(nameof(SetDoubleExpTimeAsync));

            var cd = new RiftCooldowns
            {
                UserId = userId,
                DoubleExpTime = time,
            };

            await using var context = new RiftContext();
            context.Attach(cd).Property(x => x.DoubleExpTime).IsModified = true;
            await context.SaveChangesAsync();
        }

        public async Task SetBotRespeсtTimeAsync(ulong userId, DateTime time)
        {
            if (!await EnsureExistsAsync(userId))
                throw new DatabaseException(nameof(SetBotRespeсtTimeAsync));

            var cd = new RiftCooldowns
            {
                UserId = userId,
                BotRespectTime = time,
            };

            await using var context = new RiftContext();
            context.Attach(cd).Property(x => x.BotRespectTime).IsModified = true;
            await context.SaveChangesAsync();
        }

        public async Task SetLastBragTimeAsync(ulong userId, DateTime time)
        {
            if (!await EnsureExistsAsync(userId))
                throw new DatabaseException(nameof(SetLastBragTimeAsync));

            var cd = new RiftCooldowns
            {
                UserId = userId,
                LastBragTime = time,
            };

            await using var context = new RiftContext();
            context.Attach(cd).Property(x => x.LastBragTime).IsModified = true;
            await context.SaveChangesAsync();
        }

        public async Task SetLastGiftTimeAsync(ulong userId, DateTime time)
        {
            if (!await EnsureExistsAsync(userId))
                throw new DatabaseException(nameof(SetLastGiftTimeAsync));

            var cd = new RiftCooldowns
            {
                UserId = userId,
                LastGiftTime = time,
            };

            await using var context = new RiftContext();
            context.Attach(cd).Property(x => x.LastGiftTime).IsModified = true;
            await context.SaveChangesAsync();
        }
        
        public async Task SetLastCommunityVoteTimeAsync(ulong userId, DateTime time)
        {
            if (!await EnsureExistsAsync(userId))
                throw new DatabaseException(nameof(SetLastCommunityVoteTimeAsync));

            var cd = new RiftCooldowns
            {
                UserId = userId,
                LastCommunityVoteTime = time,
            };

            await using var context = new RiftContext();
            context.Attach(cd).Property(x => x.LastCommunityVoteTime).IsModified = true;
            await context.SaveChangesAsync();
        }

        public async Task SetLastStreamerVoteTimeAsync(ulong userId, DateTime time)
        {
            if (!await EnsureExistsAsync(userId))
                throw new DatabaseException(nameof(SetLastStreamerVoteTimeAsync));

            var cd = new RiftCooldowns
            {
                UserId = userId,
                LastStreamerVoteTime = time,
            };

            await using var context = new RiftContext();
            context.Attach(cd).Property(x => x.LastStreamerVoteTime).IsModified = true;
            await context.SaveChangesAsync();
        }

        public async Task SetLastTeamVoteTimeAsync(ulong userId, DateTime time)
        {
            if (!await EnsureExistsAsync(userId))
                throw new DatabaseException(nameof(SetLastTeamVoteTimeAsync));

            var cd = new RiftCooldowns
            {
                UserId = userId,
                LastTeamVoteTime = time,
            };

            await using var context = new RiftContext();
            context.Attach(cd).Property(x => x.LastTeamVoteTime).IsModified = true;
            await context.SaveChangesAsync();
        }

        public async Task SetLastDailyRewardTimeAsync(ulong userId, DateTime time)
        {
            if (!await EnsureExistsAsync(userId))
                throw new DatabaseException(nameof(SetLastDailyRewardTimeAsync));

            var cd = new RiftCooldowns
            {
                UserId = userId,
                LastDailyRewardTime = time,
            };

            await using var context = new RiftContext();
            context.Attach(cd).Property(x => x.LastDailyRewardTime).IsModified = true;
            await context.SaveChangesAsync();
        }
        
        public async Task ResetAsync(ulong userId)
        {
            if (!await EnsureExistsAsync(userId))
                throw new DatabaseException(nameof(ResetAsync));

            var cds = await GetAsync(userId);
            var minDate = DateTime.MinValue;

            if (cds.ItemStoreTimeSpan != TimeSpan.Zero)
                await SetLastItemStoreTimeAsync(userId, minDate);
            
            if (cds.RoleStoreTimeSpan != TimeSpan.Zero)
                await SetLastRoleStoreTimeAsync(userId, minDate);
            
            if (cds.BackgroundStoreTimeSpan != TimeSpan.Zero)
                await SetLastBackgroundStoreTimeAsync(userId, minDate);
            
            if (cds.BragTimeSpan != TimeSpan.Zero)
                await SetLastBragTimeAsync(userId, minDate);
            
            if (cds.GiftTimeSpan != TimeSpan.Zero)
                await SetLastGiftTimeAsync(userId, minDate);
        }
    }
}
