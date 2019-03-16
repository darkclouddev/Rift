﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Rift.Data;
using Rift.Data.Models;
using Rift.Data.Models.LolData;
using Rift.Data.Models.Statistics;
using Rift.Data.Models.Timestamps;
using Rift.Data.Models.Users;
using Rift.Events;

using Microsoft.EntityFrameworkCore;
using NLog.Fluent;
using Rift.Data.Models.Cooldowns;

namespace Rift.Services
{
    public class DatabaseService
    {
        public delegate void DonateAdded(DonateAddedEventArgs e);

        public event DonateAdded OnDonateAdded;

        #region Cooldowns

        static async Task<bool> EnsureCooldownsExistsAsync(ulong userId)
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

        public async Task<UserCooldowns> GetUserCooldownsAsync(ulong userId)
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
        
        public async Task<UserDoubleExpTime> GetUserDoubleExpTimeAsync(ulong userId)
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

        public async Task<UserLastStoreTime> GetUserLastStoreTimeAsync(ulong userId)
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

        public async Task<UserLastAttackTime> GetUserLastAttackTimeAsync(ulong userId)
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

        public async Task<UserLastBeingAttackedTime> GetUserLastBeingAttackedTimeAsync(ulong userId)
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

        public async Task<UserLastDailyChestTime> GetUserLastDailyChestTimeAsync(ulong userId)
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

        public async Task<UserLastBragTime> GetUserLastBragTimeAsync(ulong userId)
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

        public async Task<UserLastGiftTime> GetUserLastGiftTimeAsync(ulong userId)
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

        public async Task<UserBotRespectTime[]> GetBotRespectedUsersAsync()
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

        public async Task<UserLastLolAccountUpdateTime[]> GetTenUsersForUpdateAsync()
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

        public async Task SetLastStoreTimeAsync(ulong userId, DateTime time)
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

        public async Task SetLastAttackTimeAsync(ulong userId, DateTime time)
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

        public async Task SetLastBeingAttackedTimeAsync(ulong userId, DateTime time)
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

        public async Task SetLastDailyChestTimeAsync(ulong userId, DateTime time)
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

        public async Task SetDoubleExpTimeAsync(ulong userId, DateTime time)
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

        public async Task SetBotRespeсtTimeAsync(ulong userId, DateTime time)
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

        public async Task SetLastBragTimeAsync(ulong userId, DateTime time)
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

        public async Task SetLastGiftTimeAsync(ulong userId, DateTime time)
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

        public async Task SetLastLolAccountUpdateTimeAsync(ulong userId, DateTime time)
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

        static async Task<bool> EnsureInventoryExistsAsync(ulong userId)
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

        public async Task<UserInventory> GetUserInventoryAsync(ulong userId)
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

        public async Task<UserTickets[]> GetUsersWithUsualTicketsAsync()
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

        public async Task<UserTickets[]> GetUsersWithRareTicketsAsync()
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

        public async Task AddInventoryAsync(ulong userId, uint coins = 0u, uint tokens = 0u, uint chests = 0u,
                                            uint capsules = 0u, uint spheres = 0u, uint doubleExps = 0u,
                                            uint respects = 0u, uint usualTickets = 0u, uint rareTickets = 0u)
        {
            var dbInventory = await GetUserInventoryAsync(userId);
            var dbStatistics = await GetUserStatisticsAsync(userId);

            var inventory = new RiftInventory { UserId = userId };
            var statistics = new RiftStatistics { UserId = userId };

            using (var context = new RiftContext())
            {
                var inventoryEntry = context.Attach(inventory);
                var statisticsEntry = context.Attach(statistics);

                if (coins > uint.MinValue)
                {
                    uint coinsBefore = dbInventory.Coins;

                    if (uint.MaxValue - coinsBefore < coins)
                        inventory.Coins = uint.MaxValue;
                    else
                        inventory.Coins = coinsBefore + coins;

                    if (uint.MaxValue - dbStatistics.CoinsEarnedTotal < coins)
                        statistics.CoinsEarnedTotal = uint.MaxValue;
                    else
                        statistics.CoinsEarnedTotal = dbStatistics.CoinsEarnedTotal + coins;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s coin(s): ({coinsBefore.ToString()} => {inventory.Coins.ToString()})");

                    inventoryEntry.Property(x => x.Coins).IsModified = true;
                    statisticsEntry.Property(x => x.CoinsEarnedTotal).IsModified = true;
                }

                if (tokens > uint.MinValue)
                {
                    uint tokensBefore = dbInventory.Tokens;

                    if (uint.MaxValue - tokensBefore < tokens)
                        inventory.Tokens = uint.MaxValue;
                    else
                        inventory.Tokens = tokensBefore + tokens;

                    if (uint.MaxValue - dbStatistics.TokensEarnedTotal < tokens)
                        statistics.TokensEarnedTotal = uint.MaxValue;
                    else
                        statistics.TokensEarnedTotal = dbStatistics.TokensEarnedTotal + tokens;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s token(s): ({tokensBefore.ToString()} => {inventory.Tokens.ToString()})");

                    inventoryEntry.Property(x => x.Tokens).IsModified = true;
                    statisticsEntry.Property(x => x.TokensEarnedTotal).IsModified = true;
                }

                if (chests > uint.MinValue)
                {
                    uint chestsBefore = dbInventory.Chests;

                    if (uint.MaxValue - chestsBefore < chests)
                        inventory.Chests = uint.MaxValue;
                    else
                        inventory.Chests = chestsBefore + chests;

                    if (uint.MaxValue - dbStatistics.ChestsEarnedTotal < chests)
                        statistics.ChestsEarnedTotal = uint.MaxValue;
                    else
                        statistics.ChestsEarnedTotal = dbStatistics.ChestsEarnedTotal + chests;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s chest(s): ({chestsBefore.ToString()} => {inventory.Chests.ToString()})");

                    inventoryEntry.Property(x => x.Chests).IsModified = true;
                    statisticsEntry.Property(x => x.ChestsEarnedTotal).IsModified = true;
                }

                if (capsules > uint.MinValue)
                {
                    uint capsulesBefore = dbInventory.Capsules;

                    if (uint.MaxValue - capsulesBefore < capsules)
                        inventory.Capsules = uint.MaxValue;
                    else
                        inventory.Capsules = capsulesBefore + capsules;

                    if (uint.MaxValue - dbStatistics.CapsuleEarnedTotal < capsules)
                        statistics.CapsuleEarnedTotal = uint.MaxValue;
                    else
                        statistics.CapsuleEarnedTotal = dbStatistics.CapsuleEarnedTotal + capsules;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s capsule(s): ({capsulesBefore.ToString()} => {inventory.Capsules.ToString()})");

                    inventoryEntry.Property(x => x.Capsules).IsModified = true;
                    statisticsEntry.Property(x => x.CapsuleEarnedTotal).IsModified = true;
                }

                if (spheres > uint.MinValue)
                {
                    uint spheresBefore = dbInventory.Spheres;

                    if (uint.MaxValue - spheresBefore < spheres)
                        inventory.Spheres = uint.MaxValue;
                    else
                        inventory.Spheres = spheresBefore + spheres;

                    if (uint.MaxValue - dbStatistics.SphereEarnedTotal < spheres)
                        statistics.SphereEarnedTotal = uint.MaxValue;
                    else
                        statistics.SphereEarnedTotal = dbStatistics.SphereEarnedTotal + spheres;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s sphere(s): ({spheresBefore.ToString()} => {inventory.Spheres.ToString()})");

                    inventoryEntry.Property(x => x.Spheres).IsModified = true;
                    statisticsEntry.Property(x => x.SphereEarnedTotal).IsModified = true;
                }

                if (doubleExps > uint.MinValue)
                {
                    uint doubleExpsBefore = dbInventory.PowerupsDoubleExperience;

                    if (uint.MaxValue - doubleExpsBefore < doubleExps)
                        inventory.PowerupsDoubleExp = uint.MaxValue;
                    else
                        inventory.PowerupsDoubleExp = doubleExpsBefore + doubleExps;

                    inventoryEntry.Property(x => x.PowerupsDoubleExp).IsModified = true;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s doubleExp(s): ({doubleExpsBefore.ToString()} => {inventory.PowerupsDoubleExp.ToString()})");
                }

                if (respects > uint.MinValue)
                {
                    uint respectsBefore = dbInventory.PowerupsBotRespect;

                    if (uint.MaxValue - respectsBefore < respects)
                        inventory.PowerupsBotRespect = uint.MaxValue;
                    else
                        inventory.PowerupsBotRespect = respectsBefore + respects;

                    inventoryEntry.Property(x => x.PowerupsBotRespect).IsModified = true;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s respect(s): ({respectsBefore.ToString()} => {inventory.PowerupsBotRespect.ToString()})");
                }

                if (usualTickets > uint.MinValue)
                {
                    uint usualTicketsBefore = dbInventory.UsualTickets;

                    if (uint.MaxValue - usualTicketsBefore < usualTickets)
                        inventory.UsualTickets = uint.MaxValue;
                    else
                        inventory.UsualTickets = usualTicketsBefore + usualTickets;

                    inventoryEntry.Property(x => x.UsualTickets).IsModified = true;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s usual ticket(s): ({usualTicketsBefore.ToString()} => {inventory.UsualTickets.ToString()})");
                }

                if (rareTickets > uint.MinValue)
                {
                    uint rareTicketsBefore = dbInventory.RareTickets;

                    if (uint.MaxValue - rareTicketsBefore < rareTickets)
                        inventory.RareTickets = uint.MaxValue;
                    else
                        inventory.RareTickets = rareTicketsBefore + rareTickets;

                    inventoryEntry.Property(x => x.RareTickets).IsModified = true;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s rare ticket(s): ({rareTicketsBefore.ToString()} => {inventory.RareTickets.ToString()})");
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task RemoveInventoryAsync(ulong userId, uint coins = 0u, uint tokens = 0u, uint chests = 0u,
                                               uint capsules = 0u, uint spheres = 0u, uint doubleExps = 0u,
                                               uint respects = 0u, uint usualTickets = 0u, uint rareTickets = 0u)
        {
            var dbInventory = await GetUserInventoryAsync(userId);
            var dbStatistics = await GetUserStatisticsAsync(userId);

            var inventory = new RiftInventory { UserId = userId };
            var statistics = new RiftStatistics { UserId = userId };

            using (var context = new RiftContext())
            {
                var inventoryEntry = context.Attach(inventory);
                var statisticsEntry = context.Attach(statistics);

                uint coinsBefore = dbInventory.Coins;
                coins = Math.Min(coins, coinsBefore);
                if (coins > uint.MinValue)
                {
                    inventory.Coins = coinsBefore - coins;

                    if (uint.MaxValue - dbInventory.CoinsSpentTotal < coins)
                        statistics.CoinsSpentTotal = uint.MaxValue;
                    else
                        statistics.CoinsSpentTotal = dbInventory.CoinsSpentTotal + coins;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s coin(s): ({coinsBefore.ToString()} => {inventory.Coins.ToString()})");

                    inventoryEntry.Property(x => x.Coins).IsModified = true;
                    statisticsEntry.Property(x => x.CoinsSpentTotal).IsModified = true;
                }

                uint tokensBefore = dbInventory.Tokens;
                tokens = Math.Min(tokens, tokensBefore);
                if (tokens > uint.MinValue)
                {
                    inventory.Tokens = tokensBefore - tokens;

                    if (uint.MaxValue - dbStatistics.TokensSpentTotal < tokens)
                        statistics.TokensSpentTotal = uint.MaxValue;
                    else
                        statistics.TokensSpentTotal = dbStatistics.TokensSpentTotal + tokens;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s token(s): ({tokensBefore.ToString()} => {inventory.Tokens.ToString()})");

                    inventoryEntry.Property(x => x.Tokens).IsModified = true;
                    statisticsEntry.Property(x => x.TokensSpentTotal).IsModified = true;
                }

                uint chestsBefore = dbInventory.Chests;
                chests = Math.Min(chests, chestsBefore);
                if (chests > uint.MinValue)
                {
                    inventory.Chests = chestsBefore - chests;
                    RiftBot.Log.Info($"Modified {userId.ToString()}'s chest(s): ({chestsBefore.ToString()} => {inventory.Chests.ToString()})");
                    inventoryEntry.Property(x => x.Chests).IsModified = true;
                }

                uint capsulesBefore = dbInventory.Capsules;
                capsules = Math.Min(capsules, capsulesBefore);
                if (capsules > uint.MinValue)
                {
                    inventory.Capsules = capsulesBefore - capsules;
                    RiftBot.Log.Info($"Modified {userId.ToString()}'s capsule(s): ({capsulesBefore.ToString()} => {inventory.Capsules.ToString()})");
                    inventoryEntry.Property(x => x.Capsules).IsModified = true;
                }

                uint spheresBefore = dbInventory.Spheres;
                spheres = Math.Min(spheres, spheresBefore);
                if (spheres > uint.MinValue)
                {
                    inventory.Spheres = spheresBefore - spheres;
                    RiftBot.Log.Info($"Modified {userId.ToString()}'s sphere(s): ({spheresBefore.ToString()} => {inventory.Spheres.ToString()})");
                    inventoryEntry.Property(x => x.Spheres).IsModified = true;
                }

                uint doubleExpsBefore = dbInventory.PowerupsDoubleExperience;
                doubleExps = Math.Min(doubleExps, doubleExpsBefore);
                if (doubleExps > uint.MinValue)
                {
                    inventory.PowerupsDoubleExp = doubleExpsBefore - doubleExps;
                    inventoryEntry.Property(x => x.PowerupsDoubleExp).IsModified = true;
                    RiftBot.Log.Info($"Modified {userId.ToString()}'s doubleExp(s): ({doubleExpsBefore.ToString()} => {inventory.PowerupsDoubleExp.ToString()})");
                }

                uint respectsBefore = dbInventory.PowerupsBotRespect;
                respects = Math.Min(respects, respectsBefore);
                if (respects > uint.MinValue)
                {
                    inventory.PowerupsBotRespect = respectsBefore - respects;
                    inventoryEntry.Property(x => x.PowerupsBotRespect).IsModified = true;
                    RiftBot.Log.Info($"Modified {userId.ToString()}'s respect(s): ({respectsBefore.ToString()} => {inventory.PowerupsBotRespect.ToString()})");
                }

                uint usualTicketsBefore = dbInventory.UsualTickets;
                usualTickets = Math.Min(usualTickets, usualTicketsBefore);
                if (usualTickets > uint.MinValue)
                {
                    inventory.UsualTickets = usualTicketsBefore - usualTickets;
                    inventoryEntry.Property(x => x.UsualTickets).IsModified = true;
                    RiftBot.Log.Info($"Modified {userId.ToString()}'s usual ticket(s): ({usualTicketsBefore.ToString()} => {inventory.UsualTickets.ToString()})");
                }

                uint rareTicketsBefore = dbInventory.RareTickets;
                rareTickets = Math.Min(rareTickets, rareTicketsBefore);
                if (rareTickets > uint.MinValue)
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

        public async Task<RiftLolData> GetUserLolDataAsync(ulong userId)
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

        public async Task AddLolDataAsync(RiftLolData lolData)
        {
            await EnsureUserExistsAsync(lolData.UserId);
            
            using (var context = new RiftContext())
            {
                await context.LolData.AddAsync(lolData);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateLolDataAsync(ulong userId, string region, string playerUuid, string accountId,
                                          string summonerId, string summonerName)
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

        public async Task RemoveLolDataAsync(ulong userId)
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

        public async Task<bool> HasLolDataAsync(ulong userId)
        {
            var result = await GetUserLolDataAsync(userId);

            return !(result is null) && !String.IsNullOrWhiteSpace(result.PlayerUUID);
        }

        public async Task<bool> IsTakenAsync(string region, string playerUUID)
        {
            using (var context = new RiftContext())
            {
                return await context.LolData.AnyAsync(x => x.PlayerUUID == playerUUID
                                                           && x.SummonerRegion == region);
            }
        }

        #endregion LolData

        #region Pending Users

        public async Task AddPendingUserAsync(RiftPendingUser pendingUser)
        {
            await EnsureUserExistsAsync(pendingUser.UserId);

            using (var context = new RiftContext())
            {
                await context.PendingUsers.AddAsync(pendingUser);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<PendingUser>> GetAllPendingUsersAsync()
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

        public async Task<RiftPendingUser> GetPendingUserAsync(ulong userId)
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

        public async Task RemovePendingUserAsync(PendingUser pendingUser)
        {
            await RemovePendingUserAsync(pendingUser.UserId);
        }

        public async Task RemovePendingUserAsync(ulong userId)
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

        public async Task<bool> IsPendingAsync(ulong userId)
        {
            using (var context = new RiftContext())
            {
                return await context.PendingUsers.AnyAsync(x => x.UserId == userId);
            }
        }

        public async Task<List<RiftPendingUser>> GetExpiredPendingUsersAsync()
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

        static async Task<bool> EnsureStatisticsExistsAsync(ulong userId)
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

        public async Task<UserStatistics> GetUserStatisticsAsync(UInt64 userId)
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

        public async Task AddStatisticsAsync(ulong userId, ulong giftsSent = 0ul, ulong giftsReceived = 0ul,
                                             ulong bragTotal = 0ul, ulong chestsOpenedTotal = 0ul,
                                             ulong sphereOpenedTotal = 0ul,
                                             ulong capsuleOpenedTotal = 0ul, ulong attacksDone = 0ul,
                                             ulong attacksReceived = 0ul,
                                             ulong messagesSentTotal = 0ul, ulong purchasedItemsTotal = 0ul)
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

                if (giftsSent > ulong.MinValue)
                {
                    ulong before = statistics.GiftsSent;

                    if (ulong.MaxValue - before < giftsSent)
                        statUser.GiftsSent = ulong.MaxValue;
                    else
                        statUser.GiftsSent = before + giftsSent;

                    entry.Property(x => x.GiftsSent).IsModified = true;
                }

                if (giftsReceived > ulong.MinValue)
                {
                    ulong before = statistics.GiftsReceived;

                    if (ulong.MaxValue - before < giftsReceived)
                        statUser.GiftsReceived = ulong.MaxValue;
                    else
                        statUser.GiftsReceived = before + giftsReceived;

                    entry.Property(x => x.GiftsReceived).IsModified = true;
                }

                if (bragTotal > ulong.MinValue)
                {
                    ulong before = statistics.BragTotal;

                    if (ulong.MaxValue - before < bragTotal)
                        statUser.BragTotal = ulong.MaxValue;
                    else
                        statUser.BragTotal = before + bragTotal;

                    entry.Property(x => x.BragTotal).IsModified = true;
                }

                if (chestsOpenedTotal > ulong.MinValue)
                {
                    ulong before = statistics.ChestsOpenedTotal;

                    if (ulong.MaxValue - before < chestsOpenedTotal)
                        statUser.ChestsOpenedTotal = ulong.MaxValue;
                    else
                        statUser.ChestsOpenedTotal = before + chestsOpenedTotal;

                    entry.Property(x => x.ChestsOpenedTotal).IsModified = true;
                }

                if (sphereOpenedTotal > ulong.MinValue)
                {
                    ulong before = statistics.SphereOpenedTotal;

                    if (ulong.MaxValue - before < sphereOpenedTotal)
                        statUser.SphereOpenedTotal = ulong.MaxValue;
                    else
                        statUser.SphereOpenedTotal = before + sphereOpenedTotal;

                    entry.Property(x => x.SphereOpenedTotal).IsModified = true;
                }

                if (capsuleOpenedTotal > ulong.MinValue)
                {
                    ulong before = statistics.CapsuleOpenedTotal;

                    if (ulong.MaxValue - before < capsuleOpenedTotal)
                        statUser.CapsuleOpenedTotal = ulong.MaxValue;
                    else
                        statUser.CapsuleOpenedTotal = before + capsuleOpenedTotal;

                    entry.Property(x => x.CapsuleOpenedTotal).IsModified = true;
                }

                if (attacksDone > ulong.MinValue)
                {
                    ulong before = statistics.AttacksDone;

                    if (ulong.MaxValue - before < attacksDone)
                        statUser.AttacksDone = ulong.MaxValue;
                    else
                        statUser.AttacksDone = before + attacksDone;

                    entry.Property(x => x.AttacksDone).IsModified = true;
                }

                if (attacksReceived > ulong.MinValue)
                {
                    ulong before = statistics.AttacksReceived;

                    if (ulong.MaxValue - before < attacksReceived)
                        statUser.AttacksReceived = ulong.MaxValue;
                    else
                        statUser.AttacksReceived = before + attacksReceived;

                    entry.Property(x => x.AttacksReceived).IsModified = true;
                }

                if (messagesSentTotal > ulong.MinValue)
                {
                    ulong before = statistics.MessagesSentTotal;

                    if (ulong.MaxValue - before < messagesSentTotal)
                        statUser.MessagesSentTotal = ulong.MaxValue;
                    else
                        statUser.MessagesSentTotal = before + messagesSentTotal;

                    entry.Property(x => x.MessagesSentTotal).IsModified = true;
                }

                if (purchasedItemsTotal > ulong.MinValue)
                {
                    ulong before = statistics.PurchasedItemsTotal;

                    if (ulong.MaxValue - before < purchasedItemsTotal)
                        statUser.PurchasedItemsTotal = ulong.MaxValue;
                    else
                        statUser.PurchasedItemsTotal = before + purchasedItemsTotal;

                    entry.Property(x => x.PurchasedItemsTotal).IsModified = true;
                }

                await context.SaveChangesAsync();
            }
        }

        #endregion Statistics

        #region Users

        static async Task<bool> EnsureUserExistsAsync(ulong userId)
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

        public async Task<int> GetUserCountAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.Users.CountAsync();
            }
        }

        public async Task<UserProfile> GetUserProfileAsync(ulong userId)
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

        async Task<UserDonate> GetUserDonateAsync(ulong userId)
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

        public async Task<UserLevel> GetUserLevelAsync(ulong userId)
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

        public async Task<UserTopCoins[]> GetTopTenByCoinsAsync(Func<UserTopCoins, bool> predicate)
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

        public async Task<UserTopExp[]> GetTopTenByExpAsync(Func<UserTopExp, bool> predicate)
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

        public async Task<UserDonate[]> GetTopTenDonatesAsync(Func<UserDonate, bool> predicate)
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

        public async Task SetLevelAsync(ulong userId, uint level)
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

        public async Task RemoveUserAsync(ulong userId)
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

        public async Task AddDonateAsync(ulong userId, decimal amount)
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

                decimal donateBefore = dbDonate.Donate;

                if (decimal.MaxValue - donateBefore < amount)
                    user.Donate = decimal.MaxValue;
                else
                    user.Donate = donateBefore + amount;

                context.Attach(user).Property(x => x.Donate).IsModified = true;

                await context.SaveChangesAsync();

                OnDonateAdded?.Invoke(new DonateAddedEventArgs(userId, amount, donateBefore, user.Donate));
            }
        }

        public async Task RemoveDonateAsync(ulong userId, decimal amount)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new DatabaseException(nameof(RemoveDonateAsync));

            var dbDonate = await GetUserDonateAsync(userId);

            if (dbDonate.Donate == 0M)
                return;

            using (var context = new RiftContext())
            {
                decimal donateBefore = dbDonate.Donate;

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

                    RiftBot.Log.Info($"Modified {userId}'s donate ({donateBefore} => {user.Donate})");
                }
            }
        }

        public async Task AddExperienceAsync(ulong userId, uint exp = 0u)
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

                uint expBefore = profile.Experience;

                if (uint.MaxValue - expBefore < exp)
                    dbUser.Experience = uint.MaxValue;
                else
                    dbUser.Experience = expBefore + exp;

                if (exp > 2)
                    RiftBot.Log.Info($"Modified {userId}'s exp(s): ({expBefore} => {dbUser.Experience})");

                entry.Property(x => x.Experience).IsModified = true;

                await context.SaveChangesAsync();
            }
        }

        #endregion Users

        #region Scheduled Events

        public async Task<List<ScheduledEvent>> GetAllEventsAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.ScheduledEvents.ToListAsync();
            }
        }

        public async Task<List<ScheduledEvent>> GetEventsAsync(Expression<Func<ScheduledEvent, bool>> predicate)
        {
            using (var context = new RiftContext())
            {
                return await context.ScheduledEvents.Where(predicate).ToListAsync();
            }
        }

        #endregion Scheduled Events

        #region Temp Roles

        public async Task AddTempRoleAsync(RiftTempRole role)
        {
            await EnsureUserExistsAsync(role.UserId);
            
            using (var context = new RiftContext())
            {
                await context.TempRoles.AddAsync(role);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<RiftTempRole>> GetUserTempRolesAsync(ulong userId)
        {
            using (var context = new RiftContext())
            {
                return await context.TempRoles
                                    .Where(x => x.UserId == userId)
                                    .ToListAsync();
            }
        }

        public async Task<RiftTempRole> GetNearestExpiringTempRoleAsync()
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

        public async Task<List<RiftTempRole>> GetExpiredTempRolesAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.TempRoles
                                    .Where(x => x.ExpirationTime <= DateTime.UtcNow)
                                    .ToListAsync();
            }
        }

        public async Task<int> GetTempRolesCountAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.TempRoles.CountAsync();
            }
        }

        public async Task<bool> HasTempRoleAsync(ulong userId, ulong roleId)
        {
            using (var context = new RiftContext())
            {
                return await context.TempRoles.AnyAsync(x => x.UserId == userId && x.RoleId == roleId);
            }
        }

        public async Task RemoveUserTempRoleAsync(ulong userId, ulong roleId)
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
        
        public async Task<List<RiftStreamer>> GetAllStreamersAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.Streamers.ToListAsync();
            }
        }
        
        public async Task<RiftStreamer> GetStreamer(UInt64 userId)
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
        public readonly string Reason;

        public DatabaseException(string message) : base(message)
        {
            Reason = message;
        }
    }
}
