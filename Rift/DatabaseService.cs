using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Rift.Data;
using Rift.Data.Models;
using Rift.Data.Models.Cooldowns;
using Rift.Data.Models.LolData;
using Rift.Data.Models.Statistics;
using Rift.Data.Models.Users;

using Microsoft.EntityFrameworkCore;

namespace Rift
{
    public static class Database
    {
        #region Cooldowns

        static async Task<Boolean> EnsureCooldownsExistsAsync(UInt64 userId)
        {
            if (!await EnsureUserExistsAsync(userId))
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
                    RiftBot.Log.Error($"Failed to check {nameof(EnsureCooldownsExistsAsync)} for user {userId.ToString()}.");
                    RiftBot.Log.Error(ex);
                    return false;
                }
            }
        }

        public static async Task<UserCooldowns> GetUserCooldownsAsync(UInt64 userId)
        {
            if (!await EnsureCooldownsExistsAsync(userId))
                throw new DatabaseException(nameof(GetUserDoubleExpTimeAsync));

            using (var context = new RiftContext())
            {
                return await context.Cooldowns
                    .Where(x => x.UserId == userId)
                    .Select(x => new UserCooldowns(x))
                    .FirstAsync();
            }
        }
        
        public static async Task<UserDoubleExpTime> GetUserDoubleExpTimeAsync(UInt64 userId)
        {
            if (!await EnsureCooldownsExistsAsync(userId))
                throw new DatabaseException(nameof(GetUserDoubleExpTimeAsync));

            using (var context = new RiftContext())
            {
                return await context.Cooldowns
                    .Where(x => x.UserId == userId)
                    .Select(x => new UserDoubleExpTime
                    {
                        UserId = x.UserId,
                        DoubleExpTime = x.DoubleExpTime,
                    })
                    .FirstAsync();
            }
        }

        public static async Task<UserLastStoreTime> GetUserLastStoreTimeAsync(UInt64 userId)
        {
            if (!await EnsureCooldownsExistsAsync(userId))
                throw new DatabaseException(nameof(GetUserLastStoreTimeAsync));

            using (var context = new RiftContext())
            {
                return await context.Cooldowns
                    .Where(x => x.UserId == userId)
                    .Select(x => new UserLastStoreTime
                    {
                        UserId = x.UserId,
                        LastStoreTime = x.LastStoreTime,
                    })
                    .FirstAsync();
            }
        }

        public static async Task<UserLastAttackTime> GetUserLastAttackTimeAsync(UInt64 userId)
        {
            if (!await EnsureCooldownsExistsAsync(userId))
                throw new DatabaseException(nameof(GetUserLastAttackTimeAsync));

            using (var context = new RiftContext())
            {
                return await context.Cooldowns
                    .Where(x => x.UserId == userId)
                    .Select(x => new UserLastAttackTime
                    {
                        UserId = x.UserId,
                        LastAttackTime = x.LastAttackTime,
                    })
                    .FirstAsync();
            }
        }

        public static async Task<UserLastBeingAttackedTime> GetUserLastBeingAttackedTimeAsync(UInt64 userId)
        {
            if (!await EnsureCooldownsExistsAsync(userId))
                throw new DatabaseException(nameof(GetUserLastBeingAttackedTimeAsync));

            using (var context = new RiftContext())
            {
                return await context.Cooldowns
                    .Where(x => x.UserId == userId)
                    .Select(x => new UserLastBeingAttackedTime
                    {
                        UserId = x.UserId,
                        LastBeingAttackedTime = x.LastBeingAttackedTime,
                    })
                    .FirstAsync();
            }
        }

        public static async Task<UserLastDailyChestTime> GetUserLastDailyChestTimeAsync(UInt64 userId)
        {
            if (!await EnsureCooldownsExistsAsync(userId))
                throw new DatabaseException(nameof(GetUserLastDailyChestTimeAsync));

            using (var context = new RiftContext())
            {
                return await context.Cooldowns
                    .Where(x => x.UserId == userId)
                    .Select(x => new UserLastDailyChestTime
                    {
                        UserId = x.UserId,
                        LastDailyChestTime = x.LastDailyChestTime,
                    })
                    .FirstOrDefaultAsync();
            }
        }

        public static async Task<UserLastBragTime> GetUserLastBragTimeAsync(UInt64 userId)
        {
            if (!await EnsureCooldownsExistsAsync(userId))
                throw new DatabaseException(nameof(GetUserLastBragTimeAsync));

            using (var context = new RiftContext())
            {
                return await context.Cooldowns
                    .Where(x => x.UserId == userId)
                    .Select(x => new UserLastBragTime
                    {
                        UserId = x.UserId,
                        LastBragTime = x.LastBragTime,
                    })
                    .FirstAsync();
            }
        }

        public static async Task<UserLastGiftTime> GetUserLastGiftTimeAsync(UInt64 userId)
        {
            if (!await EnsureCooldownsExistsAsync(userId))
                throw new InvalidOperationException();

            using (var context = new RiftContext())
            {
                return await context.Cooldowns
                    .Where(x => x.UserId == userId)
                    .Select(x => new UserLastGiftTime
                    {
                        UserId = x.UserId,
                        LastGiftTime = x.LastGiftTime,
                    })
                    .FirstAsync();
            }
        }

        public static async Task<UserBotRespectTime[]> GetBotRespectedUsersAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.Cooldowns
                    .Select(x => new UserBotRespectTime
                    {
                        UserId = x.UserId,
                        BotRespectTime = x.BotRespectTime
                    })
                    .Where(x => x.BotRespectTime > DateTime.UtcNow)
                    .ToArrayAsync();
            }
        }

        public static async Task<UserLastLolAccountUpdateTime[]> GetTenUsersForUpdateAsync()
        {
            using (var context = new RiftContext())
            {
                var list = await context.Cooldowns
                    .Select(x => new UserLastLolAccountUpdateTime
                    {
                        UserId = x.UserId,
                        LastUpdateTime = x.LastLolAccountUpdateTime,
                    })
                    .OrderBy(x => x.LastUpdateTime)
                    .Where(x => !String.IsNullOrWhiteSpace(x.PlayerUuid))
                    .ToArrayAsync();

                return list.Take(10).ToArray();
            }
        }

        public static async Task SetLastStoreTimeAsync(UInt64 userId, DateTime time)
        {
            if (!await EnsureCooldownsExistsAsync(userId))
                throw new DatabaseException(nameof(SetLastStoreTimeAsync));

            var cd = new RiftCooldowns
            {
                UserId = userId,
                LastStoreTime = time,
            };

            using (var context = new RiftContext())
            {
                context.Attach(cd).Property(x => x.LastStoreTime).IsModified = true;
                await context.SaveChangesAsync();
            }
        }

        public static async Task SetLastAttackTimeAsync(UInt64 userId, DateTime time)
        {
            if (!await EnsureCooldownsExistsAsync(userId))
                throw new DatabaseException(nameof(SetLastAttackTimeAsync));

            var cd = new RiftCooldowns
            {
                UserId = userId,
                LastAttackTime = time,
            };

            using (var context = new RiftContext())
            {
                context.Attach(cd).Property(x => x.LastAttackTime).IsModified = true;
                await context.SaveChangesAsync();
            }
        }

        public static async Task SetLastBeingAttackedTimeAsync(UInt64 userId, DateTime time)
        {
            if (!await EnsureCooldownsExistsAsync(userId))
                throw new DatabaseException(nameof(SetLastBeingAttackedTimeAsync));

            var cd = new RiftCooldowns
            {
                UserId = userId,
                LastBeingAttackedTime = time,
            };

            using (var context = new RiftContext())
            {
                context.Attach(cd).Property(x => x.LastBeingAttackedTime).IsModified = true;
                await context.SaveChangesAsync();
            }
        }

        public static async Task SetLastDailyChestTimeAsync(UInt64 userId, DateTime time)
        {
            if (!await EnsureCooldownsExistsAsync(userId))
                throw new DatabaseException(nameof(SetLastDailyChestTimeAsync));

            var cd = new RiftCooldowns
            {
                UserId = userId,
                LastDailyChestTime = time,
            };

            using (var context = new RiftContext())
            {
                context.Attach(cd).Property(x => x.LastDailyChestTime).IsModified = true;
                await context.SaveChangesAsync();
            }
        }

        public static async Task SetDoubleExpTimeAsync(UInt64 userId, DateTime time)
        {
            if (!await EnsureCooldownsExistsAsync(userId))
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

        public static async Task SetBotRespeсtTimeAsync(UInt64 userId, DateTime time)
        {
            if (!await EnsureCooldownsExistsAsync(userId))
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

        public static async Task SetLastBragTimeAsync(UInt64 userId, DateTime time)
        {
            if (!await EnsureCooldownsExistsAsync(userId))
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

        public static async Task SetLastGiftTimeAsync(UInt64 userId, DateTime time)
        {
            if (!await EnsureCooldownsExistsAsync(userId))
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

        public static async Task SetLastLolAccountUpdateTimeAsync(UInt64 userId, DateTime time)
        {
            if (!await EnsureCooldownsExistsAsync(userId))
                throw new DatabaseException(nameof(SetLastLolAccountUpdateTimeAsync));

            var cd = new RiftCooldowns
            {
                UserId = userId,
                LastLolAccountUpdateTime = time,
            };

            using (var context = new RiftContext())
            {
                context.Attach(cd).Property(x => x.LastLolAccountUpdateTime).IsModified = true;
                await context.SaveChangesAsync();
            }
        }

        #endregion Cooldowns

        #region Inventory

        static async Task<Boolean> EnsureInventoryExistsAsync(UInt64 userId)
        {
            if (!await EnsureUserExistsAsync(userId))
                return false;
            
            using (var context = new RiftContext())
            {
                if (await context.Inventory.AnyAsync(x => x.UserId == userId))
                    return true;

                try
                {
                    var entry = new RiftInventory
                    {
                        UserId = userId,
                    };

                    await context.Inventory.AddAsync(entry);
                    await context.SaveChangesAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    RiftBot.Log.Error($"Failed to check {nameof(EnsureInventoryExistsAsync)} for user {userId.ToString()}.");
                    RiftBot.Log.Error(ex);
                    return false;
                }
            }
        }

        public static async Task<UserInventory> GetUserInventoryAsync(UInt64 userId)
        {
            if (!await EnsureInventoryExistsAsync(userId)
                || !await EnsureStatisticsExistsAsync(userId))
                throw new DatabaseException(nameof(GetUserInventoryAsync));

            using (var context = new RiftContext())
            {
                return await context.Inventory
                    .Join(context.Statistics, inv => inv.UserId, stat => stat.UserId,
                        (inventory, statistics) => new
                        {
                            Inventory = inventory,
                            Statistics = statistics
                        })
                    .Where(x => x.Inventory.UserId == userId && x.Statistics.UserId == userId)
                    .Select(x => new UserInventory
                    {
                        UserId = x.Inventory.UserId,
                        Coins = x.Inventory.Coins,
                        Tokens = x.Inventory.Tokens,
                        Chests = x.Inventory.Chests,
                        Capsules = x.Inventory.Capsules,
                        Spheres = x.Inventory.Spheres,
                        PowerupsDoubleExperience = x.Inventory.PowerupsDoubleExp,
                        PowerupsBotRespect = x.Inventory.PowerupsBotRespect,
                        CoinsEarnedTotal = x.Statistics.CoinsEarnedTotal,
                        CoinsSpentTotal = x.Statistics.CoinsSpentTotal,
                        UsualTickets = x.Inventory.UsualTickets,
                        RareTickets = x.Inventory.RareTickets,
                    })
                    .FirstOrDefaultAsync();
            }
        }

        public static async Task<UserTickets[]> GetUsersWithUsualTicketsAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.Inventory
                    .Select(x => new UserTickets
                    {
                        UserId = x.UserId,
                        UsualTickets = x.UsualTickets,
                    })
                    .Where(x => x.UsualTickets > 0)
                    .ToArrayAsync();
            }
        }

        public static async Task<UserTickets[]> GetUsersWithRareTicketsAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.Inventory
                    .Select(x => new UserTickets
                    {
                        UserId = x.UserId,
                        RareTickets = x.RareTickets,
                    })
                    .Where(x => x.RareTickets > 0)
                    .ToArrayAsync();
            }
        }

        public static async Task AddInventoryAsync(UInt64 userId, UInt32 coins = 0u, UInt32 tokens = 0u,
            UInt32 chests = 0u, UInt32 capsules = 0u, UInt32 spheres = 0u, UInt32 doubleExps = 0u,
            UInt32 respects = 0u, UInt32 usualTickets = 0u, UInt32 rareTickets = 0u)
        {
            var dbInventory = await GetUserInventoryAsync(userId);
            var dbStatistics = await GetUserStatisticsAsync(userId);

            var inventory = new RiftInventory { UserId = userId };
            var statistics = new RiftStatistics { UserId = userId };

            using (var context = new RiftContext())
            {
                var inventoryEntry = context.Attach(inventory);
                var statisticsEntry = context.Attach(statistics);

                if (coins > UInt32.MinValue)
                {
                    var coinsBefore = dbInventory.Coins;

                    if (UInt32.MaxValue - coinsBefore < coins)
                        inventory.Coins = UInt32.MaxValue;
                    else
                        inventory.Coins = coinsBefore + coins;

                    if (UInt32.MaxValue - dbStatistics.CoinsEarnedTotal < coins)
                        statistics.CoinsEarnedTotal = UInt32.MaxValue;
                    else
                        statistics.CoinsEarnedTotal = dbStatistics.CoinsEarnedTotal + coins;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s coin(s): ({coinsBefore.ToString()} => {inventory.Coins.ToString()})");

                    inventoryEntry.Property(x => x.Coins).IsModified = true;
                    statisticsEntry.Property(x => x.CoinsEarnedTotal).IsModified = true;
                }

                if (tokens > UInt32.MinValue)
                {
                    var tokensBefore = dbInventory.Tokens;

                    if (UInt32.MaxValue - tokensBefore < tokens)
                        inventory.Tokens = UInt32.MaxValue;
                    else
                        inventory.Tokens = tokensBefore + tokens;

                    if (UInt32.MaxValue - dbStatistics.TokensEarnedTotal < tokens)
                        statistics.TokensEarnedTotal = UInt32.MaxValue;
                    else
                        statistics.TokensEarnedTotal = dbStatistics.TokensEarnedTotal + tokens;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s token(s): ({tokensBefore.ToString()} => {inventory.Tokens.ToString()})");

                    inventoryEntry.Property(x => x.Tokens).IsModified = true;
                    statisticsEntry.Property(x => x.TokensEarnedTotal).IsModified = true;
                }

                if (chests > UInt32.MinValue)
                {
                    var chestsBefore = dbInventory.Chests;

                    if (UInt32.MaxValue - chestsBefore < chests)
                        inventory.Chests = UInt32.MaxValue;
                    else
                        inventory.Chests = chestsBefore + chests;

                    if (UInt32.MaxValue - dbStatistics.ChestsEarnedTotal < chests)
                        statistics.ChestsEarnedTotal = UInt32.MaxValue;
                    else
                        statistics.ChestsEarnedTotal = dbStatistics.ChestsEarnedTotal + chests;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s chest(s): ({chestsBefore.ToString()} => {inventory.Chests.ToString()})");

                    inventoryEntry.Property(x => x.Chests).IsModified = true;
                    statisticsEntry.Property(x => x.ChestsEarnedTotal).IsModified = true;
                }

                if (capsules > UInt32.MinValue)
                {
                    var capsulesBefore = dbInventory.Capsules;

                    if (UInt32.MaxValue - capsulesBefore < capsules)
                        inventory.Capsules = UInt32.MaxValue;
                    else
                        inventory.Capsules = capsulesBefore + capsules;

                    if (UInt32.MaxValue - dbStatistics.CapsuleEarnedTotal < capsules)
                        statistics.CapsuleEarnedTotal = UInt32.MaxValue;
                    else
                        statistics.CapsuleEarnedTotal = dbStatistics.CapsuleEarnedTotal + capsules;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s capsule(s): ({capsulesBefore.ToString()} => {inventory.Capsules.ToString()})");

                    inventoryEntry.Property(x => x.Capsules).IsModified = true;
                    statisticsEntry.Property(x => x.CapsuleEarnedTotal).IsModified = true;
                }

                if (spheres > UInt32.MinValue)
                {
                    var spheresBefore = dbInventory.Spheres;

                    if (UInt32.MaxValue - spheresBefore < spheres)
                        inventory.Spheres = UInt32.MaxValue;
                    else
                        inventory.Spheres = spheresBefore + spheres;

                    if (UInt32.MaxValue - dbStatistics.SphereEarnedTotal < spheres)
                        statistics.SphereEarnedTotal = UInt32.MaxValue;
                    else
                        statistics.SphereEarnedTotal = dbStatistics.SphereEarnedTotal + spheres;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s sphere(s): ({spheresBefore.ToString()} => {inventory.Spheres.ToString()})");

                    inventoryEntry.Property(x => x.Spheres).IsModified = true;
                    statisticsEntry.Property(x => x.SphereEarnedTotal).IsModified = true;
                }

                if (doubleExps > UInt32.MinValue)
                {
                    var doubleExpsBefore = dbInventory.PowerupsDoubleExperience;

                    if (UInt32.MaxValue - doubleExpsBefore < doubleExps)
                        inventory.PowerupsDoubleExp = UInt32.MaxValue;
                    else
                        inventory.PowerupsDoubleExp = doubleExpsBefore + doubleExps;

                    inventoryEntry.Property(x => x.PowerupsDoubleExp).IsModified = true;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s doubleExp(s): ({doubleExpsBefore.ToString()} => {inventory.PowerupsDoubleExp.ToString()})");
                }

                if (respects > UInt32.MinValue)
                {
                    var respectsBefore = dbInventory.PowerupsBotRespect;

                    if (UInt32.MaxValue - respectsBefore < respects)
                        inventory.PowerupsBotRespect = UInt32.MaxValue;
                    else
                        inventory.PowerupsBotRespect = respectsBefore + respects;

                    inventoryEntry.Property(x => x.PowerupsBotRespect).IsModified = true;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s respect(s): ({respectsBefore.ToString()} => {inventory.PowerupsBotRespect.ToString()})");
                }

                if (usualTickets > UInt32.MinValue)
                {
                    var usualTicketsBefore = dbInventory.UsualTickets;

                    if (UInt32.MaxValue - usualTicketsBefore < usualTickets)
                        inventory.UsualTickets = UInt32.MaxValue;
                    else
                        inventory.UsualTickets = usualTicketsBefore + usualTickets;

                    inventoryEntry.Property(x => x.UsualTickets).IsModified = true;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s usual ticket(s): ({usualTicketsBefore.ToString()} => {inventory.UsualTickets.ToString()})");
                }

                if (rareTickets > UInt32.MinValue)
                {
                    var rareTicketsBefore = dbInventory.RareTickets;

                    if (UInt32.MaxValue - rareTicketsBefore < rareTickets)
                        inventory.RareTickets = UInt32.MaxValue;
                    else
                        inventory.RareTickets = rareTicketsBefore + rareTickets;

                    inventoryEntry.Property(x => x.RareTickets).IsModified = true;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s rare ticket(s): ({rareTicketsBefore.ToString()} => {inventory.RareTickets.ToString()})");
                }

                await context.SaveChangesAsync();
            }
        }

        public static async Task RemoveInventoryAsync(UInt64 userId, UInt32 coins = 0u, UInt32 tokens = 0u,
            UInt32 chests = 0u, UInt32 capsules = 0u, UInt32 spheres = 0u, UInt32 doubleExps = 0u,
            UInt32 respects = 0u, UInt32 usualTickets = 0u, UInt32 rareTickets = 0u)
        {
            var dbInventory = await GetUserInventoryAsync(userId);
            var dbStatistics = await GetUserStatisticsAsync(userId);

            var inventory = new RiftInventory { UserId = userId };
            var statistics = new RiftStatistics { UserId = userId };

            using (var context = new RiftContext())
            {
                var inventoryEntry = context.Attach(inventory);
                var statisticsEntry = context.Attach(statistics);

                var coinsBefore = dbInventory.Coins;
                coins = Math.Min(coins, coinsBefore);
                if (coins > UInt32.MinValue)
                {
                    inventory.Coins = coinsBefore - coins;

                    if (UInt32.MaxValue - dbInventory.CoinsSpentTotal < coins)
                        statistics.CoinsSpentTotal = UInt32.MaxValue;
                    else
                        statistics.CoinsSpentTotal = dbInventory.CoinsSpentTotal + coins;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s coin(s): ({coinsBefore.ToString()} => {inventory.Coins.ToString()})");

                    inventoryEntry.Property(x => x.Coins).IsModified = true;
                    statisticsEntry.Property(x => x.CoinsSpentTotal).IsModified = true;
                }

                var tokensBefore = dbInventory.Tokens;
                tokens = Math.Min(tokens, tokensBefore);
                if (tokens > UInt32.MinValue)
                {
                    inventory.Tokens = tokensBefore - tokens;

                    if (UInt32.MaxValue - dbStatistics.TokensSpentTotal < tokens)
                        statistics.TokensSpentTotal = UInt32.MaxValue;
                    else
                        statistics.TokensSpentTotal = dbStatistics.TokensSpentTotal + tokens;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s token(s): ({tokensBefore.ToString()} => {inventory.Tokens.ToString()})");

                    inventoryEntry.Property(x => x.Tokens).IsModified = true;
                    statisticsEntry.Property(x => x.TokensSpentTotal).IsModified = true;
                }

                var chestsBefore = dbInventory.Chests;
                chests = Math.Min(chests, chestsBefore);
                if (chests > UInt32.MinValue)
                {
                    inventory.Chests = chestsBefore - chests;
                    RiftBot.Log.Info($"Modified {userId.ToString()}'s chest(s): ({chestsBefore.ToString()} => {inventory.Chests.ToString()})");
                    inventoryEntry.Property(x => x.Chests).IsModified = true;
                }

                var capsulesBefore = dbInventory.Capsules;
                capsules = Math.Min(capsules, capsulesBefore);
                if (capsules > UInt32.MinValue)
                {
                    inventory.Capsules = capsulesBefore - capsules;
                    RiftBot.Log.Info($"Modified {userId.ToString()}'s capsule(s): ({capsulesBefore.ToString()} => {inventory.Capsules.ToString()})");
                    inventoryEntry.Property(x => x.Capsules).IsModified = true;
                }

                var spheresBefore = dbInventory.Spheres;
                spheres = Math.Min(spheres, spheresBefore);
                if (spheres > UInt32.MinValue)
                {
                    inventory.Spheres = spheresBefore - spheres;
                    RiftBot.Log.Info($"Modified {userId.ToString()}'s sphere(s): ({spheresBefore.ToString()} => {inventory.Spheres.ToString()})");
                    inventoryEntry.Property(x => x.Spheres).IsModified = true;
                }

                var doubleExpsBefore = dbInventory.PowerupsDoubleExperience;
                doubleExps = Math.Min(doubleExps, doubleExpsBefore);
                if (doubleExps > UInt32.MinValue)
                {
                    inventory.PowerupsDoubleExp = doubleExpsBefore - doubleExps;
                    inventoryEntry.Property(x => x.PowerupsDoubleExp).IsModified = true;
                    RiftBot.Log.Info($"Modified {userId.ToString()}'s doubleExp(s): ({doubleExpsBefore.ToString()} => {inventory.PowerupsDoubleExp.ToString()})");
                }

                var respectsBefore = dbInventory.PowerupsBotRespect;
                respects = Math.Min(respects, respectsBefore);
                if (respects > UInt32.MinValue)
                {
                    inventory.PowerupsBotRespect = respectsBefore - respects;
                    inventoryEntry.Property(x => x.PowerupsBotRespect).IsModified = true;
                    RiftBot.Log.Info($"Modified {userId.ToString()}'s respect(s): ({respectsBefore.ToString()} => {inventory.PowerupsBotRespect.ToString()})");
                }

                var usualTicketsBefore = dbInventory.UsualTickets;
                usualTickets = Math.Min(usualTickets, usualTicketsBefore);
                if (usualTickets > UInt32.MinValue)
                {
                    inventory.UsualTickets = usualTicketsBefore - usualTickets;
                    inventoryEntry.Property(x => x.UsualTickets).IsModified = true;
                    RiftBot.Log.Info($"Modified {userId.ToString()}'s usual ticket(s): ({usualTicketsBefore.ToString()} => {inventory.UsualTickets.ToString()})");
                }

                var rareTicketsBefore = dbInventory.RareTickets;
                rareTickets = Math.Min(rareTickets, rareTicketsBefore);
                if (rareTickets > UInt32.MinValue)
                {
                    inventory.RareTickets = rareTicketsBefore - rareTickets;
                    inventoryEntry.Property(x => x.RareTickets).IsModified = true;
                    RiftBot.Log.Info($"Modified {userId.ToString()}'s rare ticket(s): ({rareTicketsBefore.ToString()} => {inventory.RareTickets.ToString()})");
                }

                await context.SaveChangesAsync();
            }
        }

        #endregion Inventory

        #region LolData

        public static async Task<RiftLolData> GetUserLolDataAsync(UInt64 userId)
        {
            using (var context = new RiftContext())
            {
                return await context.LolData
                    .Where(x => x.UserId == userId)
                    .Select(x => new RiftLolData
                    {
                        UserId = x.UserId,
                        SummonerRegion = x.SummonerRegion,
                        PlayerUUID = x.PlayerUUID,
                        AccountId = x.AccountId,
                        SummonerId = x.SummonerId,
                        SummonerName = x.SummonerName,
                    })
                    .FirstOrDefaultAsync();
            }
        }

        public static async Task AddLolDataAsync(RiftLolData lolData)
        {
            await EnsureUserExistsAsync(lolData.UserId);
            
            using (var context = new RiftContext())
            {
                await context.LolData.AddAsync(lolData);
                await context.SaveChangesAsync();
            }
        }

        public static async Task UpdateLolDataAsync(UInt64 userId, String region, String playerUuid, String accountId,
            String summonerId, String summonerName)
        {
            var dbSummoner = await GetUserLolDataAsync(userId);

            var lolData = new RiftLolData
            {
                UserId = userId,
                SummonerRegion = region,
                PlayerUUID = playerUuid,
                SummonerId = summonerId,
                SummonerName = summonerName,
            };

            using (var context = new RiftContext())
            {
                var entity = context.Attach(lolData);

                if (dbSummoner?.SummonerRegion != region)
                    entity.Property(x => x.SummonerRegion).IsModified = true;

                if (dbSummoner?.PlayerUUID != playerUuid)
                    entity.Property(x => x.PlayerUUID).IsModified = true;

                if (dbSummoner?.SummonerId != summonerId)
                    entity.Property(x => x.SummonerId).IsModified = true;

                if (dbSummoner?.SummonerName != summonerName)
                    entity.Property(x => x.SummonerName).IsModified = true;

                await context.SaveChangesAsync();
            }
        }

        public static async Task RemoveLolDataAsync(UInt64 userId)
        {
            var lolData = new RiftLolData
            {
                UserId = userId,
            };

            using (var context = new RiftContext())
            {
                context.LolData.Remove(lolData);
                await context.SaveChangesAsync();
            }
        }

        public static async Task<Boolean> HasLolDataAsync(UInt64 userId)
        {
            var result = await GetUserLolDataAsync(userId);

            return !(result is null) && !String.IsNullOrWhiteSpace(result.PlayerUUID);
        }

        public static async Task<Boolean> IsTakenAsync(String region, String playerUUID)
        {
            using (var context = new RiftContext())
            {
                return await context.LolData.AnyAsync(x => x.PlayerUUID == playerUUID
                                                           && x.SummonerRegion == region);
            }
        }

        #endregion LolData

        #region Pending Users

        public static async Task AddPendingUserAsync(RiftPendingUser pendingUser)
        {
            await EnsureUserExistsAsync(pendingUser.UserId);

            using (var context = new RiftContext())
            {
                await context.PendingUsers.AddAsync(pendingUser);
                await context.SaveChangesAsync();
            }
        }

        public static async Task<List<PendingUser>> GetAllPendingUsersAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.PendingUsers
                    .Select(x => new PendingUser
                    {
                        UserId = x.UserId,
                        Region = x.Region,
                        PlayerUUID = x.PlayerUUID,
                        AccountId = x.AccountId,
                        SummonedId = x.SummonedId,
                        ConfirmationCode = x.ConfirmationCode,
                        ExpirationTime = x.ExpirationTime,
                    })
                    .ToListAsync();
            }
        }

        public static async Task<RiftPendingUser> GetPendingUserAsync(UInt64 userId)
        {
            using (var context = new RiftContext())
            {
                return await context.PendingUsers
                    .Select(x => new RiftPendingUser
                    {
                        UserId = x.UserId,
                        Region = x.Region,
                        PlayerUUID = x.PlayerUUID,
                        AccountId = x.AccountId,
                        SummonedId = x.SummonedId,
                        ConfirmationCode = x.ConfirmationCode,
                        ExpirationTime = x.ExpirationTime
                    })
                    .Where(x => x.UserId == userId)
                    .FirstOrDefaultAsync();
            }
        }

        public static async Task RemovePendingUserAsync(PendingUser pendingUser)
        {
            await RemovePendingUserAsync(pendingUser.UserId);
        }

        public static async Task RemovePendingUserAsync(UInt64 userId)
        {
            using (var context = new RiftContext())
            {
                var pendingUser = new RiftPendingUser
                {
                    UserId = userId
                };

                context.PendingUsers.Remove(pendingUser);
                await context.SaveChangesAsync();
            }
        }

        public static async Task<Boolean> IsPendingAsync(UInt64 userId)
        {
            using (var context = new RiftContext())
            {
                return await context.PendingUsers.AnyAsync(x => x.UserId == userId);
            }
        }

        public static async Task<List<RiftPendingUser>> GetExpiredPendingUsersAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.PendingUsers
                    .Where(x => x.ExpirationTime > DateTime.UtcNow)
                    .ToListAsync();
            }
        }

        #endregion Pending Users

        #region Statistics

        static async Task<Boolean> EnsureStatisticsExistsAsync(UInt64 userId)
        {
            if (!await EnsureUserExistsAsync(userId))
                return false;
            
            using (var context = new RiftContext())
            {
                if (await context.Statistics.AnyAsync(x => x.UserId == userId))
                    return true;

                try
                {
                    var entry = new RiftStatistics
                    {
                        UserId = userId,
                    };

                    await context.Statistics.AddAsync(entry);
                    await context.SaveChangesAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    RiftBot.Log.Error($"Failed to check {nameof(EnsureStatisticsExistsAsync)} for user {userId.ToString()}.");
                    RiftBot.Log.Error(ex);
                    return false;
                }
            }
        }

        public static async Task<UserStatistics> GetUserStatisticsAsync(UInt64 userId)
        {
            if (!await EnsureStatisticsExistsAsync(userId))
                throw new DatabaseException(nameof(GetUserStatisticsAsync));
            
            using (var context = new RiftContext())
            {
                return await context.Statistics
                    .Where(x => x.UserId == userId)
                    .Select(x => new UserStatistics
                    {
                        UserId = x.UserId,
                        CoinsEarnedTotal = x.CoinsEarnedTotal,
                        TokensEarnedTotal = x.TokensEarnedTotal,
                        ChestsEarnedTotal = x.ChestsEarnedTotal,
                        SphereEarnedTotal = x.SphereEarnedTotal,
                        CapsuleEarnedTotal = x.CapsuleEarnedTotal,
                        ChestsOpenedTotal = x.ChestsOpenedTotal,
                        SphereOpenedTotal = x.SphereEarnedTotal,
                        CapsuleOpenedTotal = x.CapsuleEarnedTotal,
                        AttacksDone = x.AttacksDone,
                        AttacksReceived = x.AttacksReceived,
                        CoinsSpentTotal = x.CoinsSpentTotal,
                        TokensSpentTotal = x.TokensSpentTotal,
                        GiftsSent = x.GiftsSent,
                        GiftsReceived = x.GiftsReceived,
                        MessagesSentTotal = x.MessagesSentTotal,
                        BragTotal = x.BragTotal,
                        PurchasedItemsTotal = x.PurchasedItemsTotal,
                    })
                    .FirstAsync();
            }
        }

        public static async Task AddStatisticsAsync(UInt64 userId, UInt32 giftsSent = 0u, UInt32 giftsReceived = 0u,
            UInt32 bragTotal = 0u, UInt32 chestsOpenedTotal = 0u, UInt32 sphereOpenedTotal = 0u,
            UInt32 capsuleOpenedTotal = 0u, UInt32 attacksDone = 0u, UInt32 attacksReceived = 0u, 
            UInt32 messagesSentTotal = 0u, UInt32 purchasedItemsTotal = 0u)
        {
            if (!await EnsureStatisticsExistsAsync(userId))
                throw new DatabaseException(nameof(AddStatisticsAsync));

            var statistics = await GetUserStatisticsAsync(userId);

            var statUser = new RiftStatistics
            {
                UserId = userId
            };

            using (var context = new RiftContext())
            {
                var entry = context.Attach(statUser);

                if (giftsSent > UInt32.MinValue)
                {
                    var before = statistics.GiftsSent;

                    if (UInt32.MaxValue - before < giftsSent)
                        statUser.GiftsSent = UInt32.MaxValue;
                    else
                        statUser.GiftsSent = before + giftsSent;

                    entry.Property(x => x.GiftsSent).IsModified = true;
                }

                if (giftsReceived > UInt32.MinValue)
                {
                    var before = statistics.GiftsReceived;

                    if (UInt32.MaxValue - before < giftsReceived)
                        statUser.GiftsReceived = UInt32.MaxValue;
                    else
                        statUser.GiftsReceived = before + giftsReceived;

                    entry.Property(x => x.GiftsReceived).IsModified = true;
                }

                if (bragTotal > UInt32.MinValue)
                {
                    var before = statistics.BragTotal;

                    if (UInt32.MaxValue - before < bragTotal)
                        statUser.BragTotal = UInt32.MaxValue;
                    else
                        statUser.BragTotal = before + bragTotal;

                    entry.Property(x => x.BragTotal).IsModified = true;
                }

                if (chestsOpenedTotal > UInt32.MinValue)
                {
                    var before = statistics.ChestsOpenedTotal;

                    if (UInt32.MaxValue - before < chestsOpenedTotal)
                        statUser.ChestsOpenedTotal = UInt32.MaxValue;
                    else
                        statUser.ChestsOpenedTotal = before + chestsOpenedTotal;

                    entry.Property(x => x.ChestsOpenedTotal).IsModified = true;
                }

                if (sphereOpenedTotal > UInt32.MinValue)
                {
                    var before = statistics.SphereOpenedTotal;

                    if (UInt32.MaxValue - before < sphereOpenedTotal)
                        statUser.SphereOpenedTotal = UInt32.MaxValue;
                    else
                        statUser.SphereOpenedTotal = before + sphereOpenedTotal;

                    entry.Property(x => x.SphereOpenedTotal).IsModified = true;
                }

                if (capsuleOpenedTotal > UInt32.MinValue)
                {
                    var before = statistics.CapsuleOpenedTotal;

                    if (UInt32.MaxValue - before < capsuleOpenedTotal)
                        statUser.CapsuleOpenedTotal = UInt32.MaxValue;
                    else
                        statUser.CapsuleOpenedTotal = before + capsuleOpenedTotal;

                    entry.Property(x => x.CapsuleOpenedTotal).IsModified = true;
                }

                if (attacksDone > UInt32.MinValue)
                {
                    var before = statistics.AttacksDone;

                    if (UInt32.MaxValue - before < attacksDone)
                        statUser.AttacksDone = UInt32.MaxValue;
                    else
                        statUser.AttacksDone = before + attacksDone;

                    entry.Property(x => x.AttacksDone).IsModified = true;
                }

                if (attacksReceived > UInt32.MinValue)
                {
                    var before = statistics.AttacksReceived;

                    if (UInt32.MaxValue - before < attacksReceived)
                        statUser.AttacksReceived = UInt32.MaxValue;
                    else
                        statUser.AttacksReceived = before + attacksReceived;

                    entry.Property(x => x.AttacksReceived).IsModified = true;
                }

                if (messagesSentTotal > UInt32.MinValue)
                {
                    var before = statistics.MessagesSentTotal;

                    if (UInt32.MaxValue - before < messagesSentTotal)
                        statUser.MessagesSentTotal = UInt32.MaxValue;
                    else
                        statUser.MessagesSentTotal = before + messagesSentTotal;

                    entry.Property(x => x.MessagesSentTotal).IsModified = true;
                }

                if (purchasedItemsTotal > UInt32.MinValue)
                {
                    var before = statistics.PurchasedItemsTotal;

                    if (UInt32.MaxValue - before < purchasedItemsTotal)
                        statUser.PurchasedItemsTotal = UInt32.MaxValue;
                    else
                        statUser.PurchasedItemsTotal = before + purchasedItemsTotal;

                    entry.Property(x => x.PurchasedItemsTotal).IsModified = true;
                }

                await context.SaveChangesAsync();
            }
        }

        #endregion Statistics

        #region Users

        static async Task<Boolean> EnsureUserExistsAsync(UInt64 userId)
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

                    return true;
                }
                catch
                {
                    RiftBot.Log.Info($"Failed to check {nameof(EnsureUserExistsAsync)} for user {userId.ToString()}.");
                    return false;
                }
            }
        }

        public static async Task<Int32> GetUserCountAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.Users.CountAsync();
            }
        }

        public static async Task<UserProfile> GetUserProfileAsync(UInt64 userId)
        {
            if (!await EnsureUserExistsAsync(userId)
                || !await EnsureCooldownsExistsAsync(userId))
                throw new DatabaseException(nameof(GetUserProfileAsync));

            using (var context = new RiftContext())
            {
                return await context.Users
                    .Join(context.Cooldowns, user => user.UserId, time => time.UserId,
                          (riftUser, riftCooldown) => new
                          {
                              User = riftUser,
                              Cooldowns = riftCooldown
                          })
                    .Where(x => x.User.UserId == userId && x.Cooldowns.UserId == userId)
                    .Select(x => new UserProfile
                    {
                        UserId = x.User.UserId,
                        Experience = x.User.Experience,
                        Level = x.User.Level,
                        TotalDonate = x.User.Donate,
                        DoubleExpTime = x.Cooldowns.DoubleExpTime,
                        BotRespectTime = x.Cooldowns.BotRespectTime,
                    })
                    .FirstOrDefaultAsync();
            }
        }

        static async Task<UserDonate> GetUserDonateAsync(UInt64 userId)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new DatabaseException(nameof(GetUserDonateAsync));

            using (var context = new RiftContext())
            {
                return await context.Users
                    .Where(x => x.UserId == userId)
                    .Select(x => new UserDonate
                    {
                        UserId = x.UserId,
                        Donate = x.Donate
                    })
                    .FirstOrDefaultAsync();
            }
        }

        public static async Task<UserLevel> GetUserLevelAsync(UInt64 userId)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new DatabaseException(nameof(GetUserLevelAsync));

            using (var context = new RiftContext())
            {
                return await context.Users
                    .Where(x => x.UserId == userId)
                    .Select(x => new UserLevel
                    {
                        UserId = x.UserId,
                        Level = x.Level,
                        Experience = x.Experience,
                    })
                    .FirstAsync();
            }
        }

        public static async Task<UserTopCoins[]> GetTopTenByCoinsAsync(Func<UserTopCoins, Boolean> predicate)
        {
            using (var context = new RiftContext())
            {
                var list = await context.Inventory
                    .OrderByDescending(x => x.Coins)
                    .Select(x => new UserTopCoins
                    {
                        UserId = x.UserId,
                        Coins = x.Coins,
                        Tokens = x.Tokens
                    })
                    .ToListAsync();

                return list.Where(predicate).Take(10).ToArray();
            }
        }

        public static async Task<UserTopExp[]> GetTopTenByExpAsync(Func<UserTopExp, Boolean> predicate)
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

                return list.Where(predicate).Take(10).ToArray();
            }
        }

        public static async Task<UserDonate[]> GetTopTenDonatesAsync(Func<UserDonate, Boolean> predicate)
        {
            using (var context = new RiftContext())
            {
                var list = await context.Users
                    .Where(x => x.Donate > 0M)
                    .OrderByDescending(x => x.Donate)
                    .Select(x => new UserDonate
                    {
                        UserId = x.UserId,
                        Donate = x.Donate,
                    })
                    .ToListAsync();

                return list.Where(predicate).Take(10).ToArray();
            }
        }

        public static async Task SetLevelAsync(UInt64 userId, UInt32 level)
        {
            if (!await EnsureUserExistsAsync(userId))
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
            }
        }

        public static async Task RemoveUserAsync(UInt64 userId)
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

        public static async Task AddDonateAsync(UInt64 userId, Decimal amount)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new DatabaseException(nameof(AddDonateAsync));

            var dbDonate = await GetUserDonateAsync(userId);

            using (var context = new RiftContext())
            {
                var user = new RiftUser
                {
                    UserId = userId
                };

                var donateBefore = dbDonate.Donate;

                if (Decimal.MaxValue - donateBefore < amount)
                    user.Donate = Decimal.MaxValue;
                else
                    user.Donate = donateBefore + amount;

                context.Attach(user).Property(x => x.Donate).IsModified = true;

                await context.SaveChangesAsync();
            }
        }

        public static async Task RemoveDonateAsync(UInt64 userId, Decimal amount)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new DatabaseException(nameof(RemoveDonateAsync));

            var dbDonate = await GetUserDonateAsync(userId);

            if (dbDonate.Donate == 0M)
                return;

            using (var context = new RiftContext())
            {
                var donateBefore = dbDonate.Donate;

                amount = Math.Min(amount, dbDonate.Donate);

                if (amount > UInt32.MinValue)
                {
                    var user = new RiftUser
                    {
                        UserId = userId,
                        Donate = donateBefore - amount
                    };

                    context.Attach(user).Property(x => x.Donate).IsModified = true;
                    await context.SaveChangesAsync();

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s donate ({donateBefore.ToString()} => {user.Donate.ToString()})");
                }
            }
        }

        public static async Task AddExperienceAsync(UInt64 userId, UInt32 exp = 0u)
        {
            if (exp == UInt32.MinValue)
                return;

            if (!await EnsureUserExistsAsync(userId))
                throw new DatabaseException(nameof(AddExperienceAsync));

            var dbUser = new RiftUser { UserId = userId };

            var profile = await GetUserProfileAsync(userId);

            using (var context = new RiftContext())
            {
                var entry = context.Attach(dbUser);

                if (DateTime.UtcNow < profile.DoubleExpTime)
                    exp *= 2;

                var expBefore = profile.Experience;

                if (UInt32.MaxValue - expBefore < exp)
                    dbUser.Experience = UInt32.MaxValue;
                else
                    dbUser.Experience = expBefore + exp;

                if (exp > 2) // TODO: wtf is that magic number
                    RiftBot.Log.Info($"Modified {userId.ToString()}'s exp(s): ({expBefore.ToString()} => {dbUser.Experience.ToString()})");

                entry.Property(x => x.Experience).IsModified = true;

                await context.SaveChangesAsync();
            }
        }

        #endregion Users

        #region Scheduled Events

        public static async Task<List<ScheduledEvent>> GetAllEventsAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.ScheduledEvents.ToListAsync();
            }
        }

        public static async Task<List<ScheduledEvent>> GetEventsAsync(Expression<Func<ScheduledEvent, Boolean>> predicate)
        {
            using (var context = new RiftContext())
            {
                return await context.ScheduledEvents.Where(predicate).ToListAsync();
            }
        }

        #endregion Scheduled Events

        #region Temp Roles

        public static async Task AddTempRoleAsync(RiftTempRole role)
        {
            await EnsureUserExistsAsync(role.UserId);
            
            using (var context = new RiftContext())
            {
                await context.TempRoles.AddAsync(role);
                await context.SaveChangesAsync();
            }
        }

        public static async Task<List<RiftTempRole>> GetUserTempRolesAsync(UInt64 userId)
        {
            using (var context = new RiftContext())
            {
                return await context.TempRoles
                    .Where(x => x.UserId == userId)
                    .ToListAsync();
            }
        }

        public static async Task<RiftTempRole> GetNearestExpiringTempRoleAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.TempRoles
                    .Where(x => x.ExpirationTime >= DateTime.UtcNow)
                    .OrderBy(x => x.ExpirationTime)
                    .Take(1)
                    .FirstOrDefaultAsync();
            }
        }

        public static async Task<List<RiftTempRole>> GetExpiredTempRolesAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.TempRoles
                    .Where(x => x.ExpirationTime <= DateTime.UtcNow)
                    .ToListAsync();
            }
        }

        public static async Task<Int32> GetTempRolesCountAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.TempRoles.CountAsync();
            }
        }

        public static async Task<Boolean> HasTempRoleAsync(UInt64 userId, UInt64 roleId)
        {
            using (var context = new RiftContext())
            {
                return await context.TempRoles.AnyAsync(x => x.UserId == userId && x.RoleId == roleId);
            }
        }

        public static async Task RemoveUserTempRoleAsync(UInt64 userId, UInt64 roleId)
        {
            var rtr = new RiftTempRole
            {
                UserId = userId,
                RoleId = roleId,
            };

            using (var context = new RiftContext())
            {
                context.TempRoles.Remove(rtr);
                await context.SaveChangesAsync();
            }
        }

        #endregion Temp Roles
        
        #region Streamers
        
        public static async Task<List<RiftStreamer>> GetAllStreamersAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.Streamers.ToListAsync();
            }
        }
        
        public static async Task<RiftStreamer> GetStreamer(UInt64 userId)
        {
            using (var context = new RiftContext())
            {
                return await context.Streamers
                    .Where(x => x.UserId == userId)
                    .FirstOrDefaultAsync();
            }
        }
        
        #endregion Streamers
    }

    public class DatabaseException : Exception
    {
        public readonly String Reason;

        public DatabaseException(String message) : base(message)
        {
            Reason = message;
        }
    }
}
