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

            using (var context = new RiftContext())
            {
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
                    RiftBot.Log.Error($"Failed to check {nameof(EnsureExistsAsync)} for user {userId.ToString()}.");
                    RiftBot.Log.Error(ex);
                    return false;
                }
            }
        }

        public async Task<RiftCooldowns> GetAsync(ulong userId)
        {
            if (!await EnsureExistsAsync(userId))
                throw new DatabaseException(nameof(GetAsync));

            using (var context = new RiftContext())
            {
                return await context.Cooldowns
                    .Where(x => x.UserId == userId)
                    .FirstAsync();
            }
        }

        public async Task<List<(ulong, DateTime)>> GetBotRespectedUsersAsync()
        {
            using (var context = new RiftContext())
            {
                var users = await context.Cooldowns
                    .Where(x => x.BotRespectTime > DateTime.UtcNow)
                    .ToListAsync();

                return users.Select(x => (x.UserId, x.BotRespectTime)).ToList();
            }
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

            using (var context = new RiftContext())
            {
                context.Attach(cd).Property(x => x.LastItemStoreTime).IsModified = true;
                await context.SaveChangesAsync();
            }
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

            using (var context = new RiftContext())
            {
                context.Attach(cd).Property(x => x.LastRoleStoreTime).IsModified = true;
                await context.SaveChangesAsync();
            }
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

            using (var context = new RiftContext())
            {
                context.Attach(cd).Property(x => x.LastBackgroundStoreTime).IsModified = true;
                await context.SaveChangesAsync();
            }
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

            using (var context = new RiftContext())
            {
                context.Attach(cd).Property(x => x.DoubleExpTime).IsModified = true;
                await context.SaveChangesAsync();
            }
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

            using (var context = new RiftContext())
            {
                context.Attach(cd).Property(x => x.BotRespectTime).IsModified = true;
                await context.SaveChangesAsync();
            }
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

            using (var context = new RiftContext())
            {
                context.Attach(cd).Property(x => x.LastBragTime).IsModified = true;
                await context.SaveChangesAsync();
            }
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

            using (var context = new RiftContext())
            {
                context.Attach(cd).Property(x => x.LastGiftTime).IsModified = true;
                await context.SaveChangesAsync();
            }
        }
    }
}
