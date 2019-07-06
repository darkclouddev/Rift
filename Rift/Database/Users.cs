using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Rift.Data;
using Rift.Data.Models;
using Rift.Data.Models.Users;
using Rift.Events;

using Microsoft.EntityFrameworkCore;

namespace Rift.Database
{
    public class Users
    {
        public EventHandler<UserCreatedEventArgs> OnUserCreated;
        public EventHandler<LevelReachedEventArgs> OnLevelReached;

        public async Task<bool> EnsureExistsAsync(ulong userId)
        {
            using (var context = new RiftContext())
            {
                if (await context.Users.AnyAsync(x => x.UserId == userId))
                    return true;

                try
                {
                    var entry = new RiftUser
                    {
                        UserId = userId,
                    };

                    await context.Users.AddAsync(entry);
                    await context.SaveChangesAsync();
                    OnUserCreated?.Invoke(null, new UserCreatedEventArgs(userId));
                    return true;
                }
                catch
                {
                    RiftBot.Log.Info($"Failed to check {nameof(EnsureExistsAsync)} for user {userId.ToString()}.");
                    return false;
                }
            }
        }

        public async Task<int> GetCountAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.Users.CountAsync();
            }
        }

        public async Task<List<ulong>> GetAllSortedAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.Users
                    .OrderByDescending(x => x.Level)
                    .ThenByDescending(x => x.Experience)
                    .Select(x => x.UserId)
                    .ToListAsync();
            }
        }

        public async Task<RiftUser> GetAsync(ulong userId)
        {
            if (!await EnsureExistsAsync(userId))
                throw new DatabaseException(nameof(GetAsync));

            using (var context = new RiftContext())
            {
                return await context.Users
                    .FirstOrDefaultAsync(x => x.UserId == userId);
            }
        }

        public async Task<List<RiftUser>> GetAsync(Expression<Func<RiftUser, bool>> predicate)
        {
            using (var context = new RiftContext())
            {
                return await context.Users
                    .Where(predicate)
                    .ToListAsync();
            }
        }

        public async Task<uint> GetLevelAsync(ulong userId)
        {
            var user = await GetAsync(userId);
            return user.Level;
        }

        public async Task<List<UserTopCoins>> GetTopTenByCoinsAsync(Func<UserTopCoins, bool> predicate)
        {
            using (var context = new RiftContext())
            {
                var list = await context.Inventory
                    .OrderByDescending(x => x.Coins)
                    .Select(x => new UserTopCoins
                    {
                        UserId = x.UserId,
                        Coins = x.Coins,
                    })
                    .ToListAsync();

                return list.Where(predicate).Take(10).ToList();
            }
        }

        public async Task<List<UserTopExp>> GetTopTenByExpAsync(Func<UserTopExp, bool> predicate)
        {
            using (var context = new RiftContext())
            {
                var list = await context.Users
                    .OrderByDescending(x => x.Experience)
                    .Select(x => new UserTopExp
                    {
                        UserId = x.UserId,
                        Level = x.Level,
                        Experience = x.Experience,
                    })
                    .ToListAsync();

                return list.Where(predicate).Take(10).ToList();
            }
        }

        public async Task SetLevelAsync(ulong userId, uint level)
        {
            if (!await EnsureExistsAsync(userId))
                throw new DatabaseException(nameof(SetLevelAsync));

            var user = new RiftUser
            {
                UserId = userId,
                Level = level,
            };

            using (var context = new RiftContext())
            {
                context.Attach(user).Property(x => x.Level).IsModified = true;
                await context.SaveChangesAsync();
                OnLevelReached?.Invoke(null, new LevelReachedEventArgs(userId, level));
            }
        }

        public async Task RemoveAsync(ulong userId)
        {
            using (var context = new RiftContext())
            {
                var dbUser = new RiftUser
                {
                    UserId = userId,
                };

                context.Users.Remove(dbUser);
                await context.SaveChangesAsync();
            }
        }

        public async Task AddExperienceAsync(ulong userId, uint exp = 0u)
        {
            if (exp == uint.MinValue)
                return;

            if (!await EnsureExistsAsync(userId))
                throw new DatabaseException(nameof(AddExperienceAsync));

            var dbUser = new RiftUser { UserId = userId };

            var profile = await GetAsync(userId);
            var cooldowns = await DB.Cooldowns.GetAsync(userId);

            using (var context = new RiftContext())
            {
                var entry = context.Attach(dbUser);

                if (DateTime.UtcNow < cooldowns.DoubleExpTime)
                    exp *= 2;

                var expBefore = profile.Experience;

                if (uint.MaxValue - expBefore < exp)
                    dbUser.Experience = uint.MaxValue;
                else
                    dbUser.Experience = expBefore + exp;

                if (exp > 2) // TODO: wtf is that magic number
                    RiftBot.Log.Info($"Modified {userId.ToString()}'s exp(s): ({expBefore.ToString()} => {dbUser.Experience.ToString()})");

                entry.Property(x => x.Experience).IsModified = true;

                await context.SaveChangesAsync();
            }
        }
    }
}
