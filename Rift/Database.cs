using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Rift.Data;
using Rift.Data.Models;
using Rift.Data.Models.Users;

using Microsoft.EntityFrameworkCore;

namespace Rift
{
    public static class Database
    {
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

        public static async Task<RiftCooldowns> GetUserCooldownsAsync(ulong userId)
        {
            if (!await EnsureCooldownsExistsAsync(userId))
                throw new DatabaseException(nameof(GetUserCooldownsAsync));

            using (var context = new RiftContext())
            {
                return await context.Cooldowns
                    .Where(x => x.UserId == userId)
                    .FirstAsync();
            }
        }
        
        public static async Task<List<(ulong, DateTime)>> GetBotRespectedUsersAsync()
        {
            using (var context = new RiftContext())
            {
                var users = await context.Cooldowns
                    .Where(x => x.BotRespectTime > DateTime.UtcNow)
                    .ToListAsync();

                return users.Select(x => (x.UserId, x.BotRespectTime)).ToList();
            }
        }

        public static async Task SetLastStoreTimeAsync(ulong userId, DateTime time)
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

        public static async Task SetLastDailyChestTimeAsync(ulong userId, DateTime time)
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

        public static async Task SetDoubleExpTimeAsync(ulong userId, DateTime time)
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

        public static async Task SetBotRespeсtTimeAsync(ulong userId, DateTime time)
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

        public static async Task SetLastBragTimeAsync(ulong userId, DateTime time)
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

        public static async Task SetLastGiftTimeAsync(ulong userId, DateTime time)
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

        public static async Task<RiftInventory> GetUserInventoryAsync(ulong userId)
        {
            if (!await EnsureInventoryExistsAsync(userId)
                || !await EnsureStatisticsExistsAsync(userId))
                throw new DatabaseException(nameof(GetUserInventoryAsync));

            using (var context = new RiftContext())
            {
                return await context.Inventory
                    .FirstOrDefaultAsync(x => x.UserId == userId);
            }
        }

        public static async Task AddInventoryAsync(ulong userId, InventoryData data)
        {
            var dbInventory = await GetUserInventoryAsync(userId);
            var dbStatistics = await GetUserStatisticsAsync(userId);

            var inventory = new RiftInventory { UserId = userId };
            var statistics = new RiftStatistics { UserId = userId };

            using (var context = new RiftContext())
            {
                var inventoryEntry = context.Attach(inventory);
                var statisticsEntry = context.Attach(statistics);

                if (data.Coins > uint.MinValue)
                {
                    var coinsBefore = dbInventory.Coins;

                    if (uint.MaxValue - coinsBefore < data.Coins)
                        inventory.Coins = uint.MaxValue;
                    else
                        inventory.Coins = coinsBefore + data.Coins;

                    if (uint.MaxValue - dbStatistics.CoinsEarnedTotal < data.Coins)
                        statistics.CoinsEarnedTotal = uint.MaxValue;
                    else
                        statistics.CoinsEarnedTotal = dbStatistics.CoinsEarnedTotal + data.Coins;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s coin(s): ({coinsBefore.ToString()} => {inventory.Coins.ToString()})");

                    inventoryEntry.Property(x => x.Coins).IsModified = true;
                    statisticsEntry.Property(x => x.CoinsEarnedTotal).IsModified = true;
                }

                if (data.Tokens > uint.MinValue)
                {
                    var tokensBefore = dbInventory.Tokens;

                    if (uint.MaxValue - tokensBefore < data.Tokens)
                        inventory.Tokens = uint.MaxValue;
                    else
                        inventory.Tokens = tokensBefore + data.Tokens;

                    if (uint.MaxValue - dbStatistics.TokensEarnedTotal < data.Tokens)
                        statistics.TokensEarnedTotal = uint.MaxValue;
                    else
                        statistics.TokensEarnedTotal = dbStatistics.TokensEarnedTotal + data.Tokens;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s token(s): ({tokensBefore.ToString()} => {inventory.Tokens.ToString()})");

                    inventoryEntry.Property(x => x.Tokens).IsModified = true;
                    statisticsEntry.Property(x => x.TokensEarnedTotal).IsModified = true;
                }

                if (data.Chests > uint.MinValue)
                {
                    var chestsBefore = dbInventory.Chests;

                    if (uint.MaxValue - chestsBefore < data.Chests)
                        inventory.Chests = uint.MaxValue;
                    else
                        inventory.Chests = chestsBefore + data.Chests;

                    if (uint.MaxValue - dbStatistics.ChestsEarnedTotal < data.Chests)
                        statistics.ChestsEarnedTotal = uint.MaxValue;
                    else
                        statistics.ChestsEarnedTotal = dbStatistics.ChestsEarnedTotal + data.Chests;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s chest(s): ({chestsBefore.ToString()} => {inventory.Chests.ToString()})");

                    inventoryEntry.Property(x => x.Chests).IsModified = true;
                    statisticsEntry.Property(x => x.ChestsEarnedTotal).IsModified = true;
                }

                if (data.Spheres > uint.MinValue)
                {
                    var spheresBefore = dbInventory.Spheres;

                    if (uint.MaxValue - spheresBefore < data.Spheres)
                        inventory.Spheres = uint.MaxValue;
                    else
                        inventory.Spheres = spheresBefore + data.Spheres;

                    if (uint.MaxValue - dbStatistics.SphereEarnedTotal < data.Spheres)
                        statistics.SphereEarnedTotal = uint.MaxValue;
                    else
                        statistics.SphereEarnedTotal = dbStatistics.SphereEarnedTotal + data.Spheres;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s sphere(s): ({spheresBefore.ToString()} => {inventory.Spheres.ToString()})");

                    inventoryEntry.Property(x => x.Spheres).IsModified = true;
                    statisticsEntry.Property(x => x.SphereEarnedTotal).IsModified = true;
                }

                if (data.Capsules > uint.MinValue)
                {
                    var capsulesBefore = dbInventory.Capsules;

                    if (uint.MaxValue - capsulesBefore < data.Capsules)
                        inventory.Capsules = uint.MaxValue;
                    else
                        inventory.Capsules = capsulesBefore + data.Capsules;

                    if (uint.MaxValue - dbStatistics.CapsuleEarnedTotal < data.Capsules)
                        statistics.CapsuleEarnedTotal = uint.MaxValue;
                    else
                        statistics.CapsuleEarnedTotal = dbStatistics.CapsuleEarnedTotal + data.Capsules;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s capsule(s): ({capsulesBefore.ToString()} => {inventory.Capsules.ToString()})");

                    inventoryEntry.Property(x => x.Capsules).IsModified = true;
                    statisticsEntry.Property(x => x.CapsuleEarnedTotal).IsModified = true;
                }

                if (data.BotRespects > uint.MinValue)
                {
                    var doubleExpsBefore = dbInventory.BonusDoubleExp;

                    if (uint.MaxValue - doubleExpsBefore < data.BotRespects)
                        inventory.BonusDoubleExp = uint.MaxValue;
                    else
                        inventory.BonusDoubleExp = doubleExpsBefore + data.BotRespects;

                    inventoryEntry.Property(x => x.BonusDoubleExp).IsModified = true;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s doubleExp(s): ({doubleExpsBefore.ToString()} => {inventory.BonusDoubleExp.ToString()})");
                }

                if (data.BotRespects > uint.MinValue)
                {
                    var respectsBefore = dbInventory.BonusBotRespect;

                    if (uint.MaxValue - respectsBefore < data.BotRespects)
                        inventory.BonusBotRespect = uint.MaxValue;
                    else
                        inventory.BonusBotRespect = respectsBefore + data.BotRespects;

                    inventoryEntry.Property(x => x.BonusBotRespect).IsModified = true;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s respect(s): ({respectsBefore.ToString()} => {inventory.BonusBotRespect.ToString()})");
                }

                if (data.Rewinds > uint.MinValue)
                {
                    var rewindsBefore = dbInventory.BonusRewind;

                    if (uint.MaxValue - rewindsBefore < data.Rewinds)
                        inventory.BonusRewind = uint.MaxValue;
                    else
                        inventory.BonusRewind = rewindsBefore + data.Rewinds;

                    inventoryEntry.Property(x => x.BonusRewind).IsModified = true;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s rewind(s): ({rewindsBefore.ToString()} => {inventory.BonusRewind.ToString()})");
                }

                await context.SaveChangesAsync();
            }
        }

        public static async Task RemoveInventoryAsync(ulong userId, InventoryData data)
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
                data.Coins = Math.Min(data.Coins, coinsBefore);
                if (data.Coins > uint.MinValue)
                {
                    inventory.Coins = coinsBefore - data.Coins;

                    if (uint.MaxValue - dbStatistics.CoinsSpentTotal < data.Coins)
                        statistics.CoinsSpentTotal = uint.MaxValue;
                    else
                        statistics.CoinsSpentTotal = dbStatistics.CoinsSpentTotal + data.Coins;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s coin(s): ({coinsBefore.ToString()} => {inventory.Coins.ToString()})");

                    inventoryEntry.Property(x => x.Coins).IsModified = true;
                    statisticsEntry.Property(x => x.CoinsSpentTotal).IsModified = true;
                }

                var tokensBefore = dbInventory.Tokens;
                data.Tokens = Math.Min(data.Tokens, tokensBefore);
                if (data.Tokens > uint.MinValue)
                {
                    inventory.Tokens = tokensBefore - data.Tokens;

                    if (uint.MaxValue - dbStatistics.TokensSpentTotal < data.Tokens)
                        statistics.TokensSpentTotal = uint.MaxValue;
                    else
                        statistics.TokensSpentTotal = dbStatistics.TokensSpentTotal + data.Tokens;

                    RiftBot.Log.Info($"Modified {userId.ToString()}'s token(s): ({tokensBefore.ToString()} => {inventory.Tokens.ToString()})");

                    inventoryEntry.Property(x => x.Tokens).IsModified = true;
                    statisticsEntry.Property(x => x.TokensSpentTotal).IsModified = true;
                }

                var chestsBefore = dbInventory.Chests;
                data.Chests = Math.Min(data.Chests, chestsBefore);
                if (data.Chests > uint.MinValue)
                {
                    inventory.Chests = chestsBefore - data.Chests;
                    RiftBot.Log.Info($"Modified {userId.ToString()}'s chest(s): ({chestsBefore.ToString()} => {inventory.Chests.ToString()})");
                    inventoryEntry.Property(x => x.Chests).IsModified = true;
                }

                var spheresBefore = dbInventory.Spheres;
                data.Spheres = Math.Min(data.Spheres, spheresBefore);
                if (data.Spheres > uint.MinValue)
                {
                    inventory.Spheres = spheresBefore - data.Spheres;
                    RiftBot.Log.Info($"Modified {userId.ToString()}'s sphere(s): ({spheresBefore.ToString()} => {inventory.Spheres.ToString()})");
                    inventoryEntry.Property(x => x.Spheres).IsModified = true;
                }

                var capsulesBefore = dbInventory.Capsules;
                data.Capsules = Math.Min(data.Capsules, capsulesBefore);
                if (data.Capsules > uint.MinValue)
                {
                    inventory.Capsules = capsulesBefore - data.Capsules;
                    RiftBot.Log.Info($"Modified {userId.ToString()}'s capsule(s): ({capsulesBefore.ToString()} => {inventory.Capsules.ToString()})");
                    inventoryEntry.Property(x => x.Capsules).IsModified = true;
                }

                var ticketsBefore = dbInventory.Tickets;
                data.Tickets = Math.Min(data.Tickets, ticketsBefore);
                if (data.Tickets > uint.MinValue)
                {
                    inventory.Tickets = ticketsBefore - data.Tickets;
                    inventoryEntry.Property(x => x.Tickets).IsModified = true;
                    RiftBot.Log.Info($"Modified {userId.ToString()}'s usual ticket(s): ({ticketsBefore.ToString()} => {inventory.Tickets.ToString()})");
                }

                var doubleExpsBefore = dbInventory.BonusDoubleExp;
                data.DoubleExps = Math.Min(data.DoubleExps, doubleExpsBefore);
                if (data.DoubleExps > uint.MinValue)
                {
                    inventory.BonusDoubleExp = doubleExpsBefore - data.DoubleExps;
                    inventoryEntry.Property(x => x.BonusDoubleExp).IsModified = true;
                    RiftBot.Log.Info($"Modified {userId.ToString()}'s doubleExp(s): ({doubleExpsBefore.ToString()} => {inventory.BonusDoubleExp.ToString()})");
                }

                var respectsBefore = dbInventory.BonusBotRespect;
                data.BotRespects = Math.Min(data.BotRespects, respectsBefore);
                if (data.BotRespects > uint.MinValue)
                {
                    inventory.BonusBotRespect = respectsBefore - data.BotRespects;
                    inventoryEntry.Property(x => x.BonusBotRespect).IsModified = true;
                    RiftBot.Log.Info($"Modified {userId.ToString()}'s respect(s): ({respectsBefore.ToString()} => {inventory.BonusBotRespect.ToString()})");
                }

                var rewindsBefore = dbInventory.BonusRewind;
                data.Rewinds = Math.Min(data.Rewinds, rewindsBefore);
                if (data.Rewinds > uint.MinValue)
                {
                    inventory.BonusRewind = rewindsBefore - data.Rewinds;
                    inventoryEntry.Property(x => x.BonusRewind).IsModified = true;
                    RiftBot.Log.Info($"Modified {userId.ToString()}'s rewind(s): ({rewindsBefore.ToString()} => {inventory.BonusRewind.ToString()})");
                }

                await context.SaveChangesAsync();
            }
        }

        #endregion Inventory

        #region LolData

        public static async Task<RiftLolData> GetUserLolDataAsync(ulong userId)
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

        public static async Task UpdateLolDataAsync(ulong userId, string region, string playerUuid, string accountId, string summonerId, string summonerName)
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

        public static async Task RemoveLolDataAsync(ulong userId)
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

        public static async Task<bool> HasLolDataAsync(ulong userId)
        {
            var result = await GetUserLolDataAsync(userId);

            return !(result is null) && !string.IsNullOrWhiteSpace(result.PlayerUUID);
        }

        public static async Task<bool> IsTakenAsync(string region, string playerUUID)
        {
            using (var context = new RiftContext())
            {
                return await context.LolData.AnyAsync(x => x.PlayerUUID == playerUUID && x.SummonerRegion == region);
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

        public static async Task<List<RiftPendingUser>> GetAllPendingUsersAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.PendingUsers
                    .ToListAsync();
            }
        }

        public static async Task<RiftPendingUser> GetPendingUserAsync(ulong userId)
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

        public static async Task RemovePendingUserAsync(RiftPendingUser pendingUser)
        {
            await RemovePendingUserAsync(pendingUser.UserId);
        }

        public static async Task RemovePendingUserAsync(ulong userId)
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

        public static async Task<bool> IsPendingAsync(ulong userId)
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

        public static async Task<RiftStatistics> GetUserStatisticsAsync(ulong userId)
        {
            if (!await EnsureStatisticsExistsAsync(userId))
                throw new DatabaseException(nameof(GetUserStatisticsAsync));
            
            using (var context = new RiftContext())
            {
                return await context.Statistics
                    .FirstAsync(x => x.UserId == userId);
            }
        }

        public static async Task AddStatisticsAsync(ulong userId, uint giftsSent = 0u, uint giftsReceived = 0u,
            uint bragTotal = 0u, uint chestsOpenedTotal = 0u, uint sphereOpenedTotal = 0u,
            uint capsuleOpenedTotal = 0u, uint attacksDone = 0u, uint attacksReceived = 0u, 
            uint messagesSentTotal = 0u, uint purchasedItemsTotal = 0u, TimeSpan? voiceUptime = null) // TODO: fix this shiet
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

                if (giftsSent > uint.MinValue)
                {
                    var before = statistics.GiftsSent;

                    if (uint.MaxValue - before < giftsSent)
                        statUser.GiftsSent = uint.MaxValue;
                    else
                        statUser.GiftsSent = before + giftsSent;

                    entry.Property(x => x.GiftsSent).IsModified = true;
                }

                if (giftsReceived > uint.MinValue)
                {
                    var before = statistics.GiftsReceived;

                    if (uint.MaxValue - before < giftsReceived)
                        statUser.GiftsReceived = uint.MaxValue;
                    else
                        statUser.GiftsReceived = before + giftsReceived;

                    entry.Property(x => x.GiftsReceived).IsModified = true;
                }

                if (bragTotal > uint.MinValue)
                {
                    var before = statistics.BragTotal;

                    if (uint.MaxValue - before < bragTotal)
                        statUser.BragTotal = uint.MaxValue;
                    else
                        statUser.BragTotal = before + bragTotal;

                    entry.Property(x => x.BragTotal).IsModified = true;
                }

                if (chestsOpenedTotal > uint.MinValue)
                {
                    var before = statistics.ChestsOpenedTotal;

                    if (uint.MaxValue - before < chestsOpenedTotal)
                        statUser.ChestsOpenedTotal = uint.MaxValue;
                    else
                        statUser.ChestsOpenedTotal = before + chestsOpenedTotal;

                    entry.Property(x => x.ChestsOpenedTotal).IsModified = true;
                }

                if (sphereOpenedTotal > uint.MinValue)
                {
                    var before = statistics.SphereOpenedTotal;

                    if (uint.MaxValue - before < sphereOpenedTotal)
                        statUser.SphereOpenedTotal = uint.MaxValue;
                    else
                        statUser.SphereOpenedTotal = before + sphereOpenedTotal;

                    entry.Property(x => x.SphereOpenedTotal).IsModified = true;
                }

                if (capsuleOpenedTotal > uint.MinValue)
                {
                    var before = statistics.CapsuleOpenedTotal;

                    if (uint.MaxValue - before < capsuleOpenedTotal)
                        statUser.CapsuleOpenedTotal = uint.MaxValue;
                    else
                        statUser.CapsuleOpenedTotal = before + capsuleOpenedTotal;

                    entry.Property(x => x.CapsuleOpenedTotal).IsModified = true;
                }

                if (attacksDone > uint.MinValue)
                {
                    var before = statistics.AttacksDone;

                    if (uint.MaxValue - before < attacksDone)
                        statUser.AttacksDone = uint.MaxValue;
                    else
                        statUser.AttacksDone = before + attacksDone;

                    entry.Property(x => x.AttacksDone).IsModified = true;
                }

                if (attacksReceived > uint.MinValue)
                {
                    var before = statistics.AttacksReceived;

                    if (uint.MaxValue - before < attacksReceived)
                        statUser.AttacksReceived = uint.MaxValue;
                    else
                        statUser.AttacksReceived = before + attacksReceived;

                    entry.Property(x => x.AttacksReceived).IsModified = true;
                }

                if (messagesSentTotal > uint.MinValue)
                {
                    var before = statistics.MessagesSentTotal;

                    if (uint.MaxValue - before < messagesSentTotal)
                        statUser.MessagesSentTotal = uint.MaxValue;
                    else
                        statUser.MessagesSentTotal = before + messagesSentTotal;

                    entry.Property(x => x.MessagesSentTotal).IsModified = true;
                }

                if (purchasedItemsTotal > uint.MinValue)
                {
                    var before = statistics.PurchasedItemsTotal;

                    if (uint.MaxValue - before < purchasedItemsTotal)
                        statUser.PurchasedItemsTotal = uint.MaxValue;
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

        public static async Task<int> GetUserCountAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.Users.CountAsync();
            }
        }

        public static async Task<RiftUser> GetUserAsync(ulong userId)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new DatabaseException(nameof(GetUserAsync));

            using (var context = new RiftContext())
            {
                return await context.Users
                    .FirstOrDefaultAsync(x => x.UserId == userId);
            }
        }

        public static async Task<uint> GetUserLevelAsync(ulong userId)
        {
            var user = await GetUserAsync(userId);
            return user.Level;
        }

        public static async Task<UserTopCoins[]> GetTopTenByCoinsAsync(Func<UserTopCoins, bool> predicate)
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

        public static async Task<UserTopExp[]> GetTopTenByExpAsync(Func<UserTopExp, bool> predicate)
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

        public static async Task SetLevelAsync(ulong userId, uint level)
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

        public static async Task RemoveUserAsync(ulong userId)
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

        public static async Task AddExperienceAsync(ulong userId, uint exp = 0u)
        {
            if (exp == uint.MinValue)
                return;

            if (!await EnsureUserExistsAsync(userId))
                throw new DatabaseException(nameof(AddExperienceAsync));

            var dbUser = new RiftUser { UserId = userId };

            var profile = await GetUserAsync(userId);
            var cooldowns = await GetUserCooldownsAsync(userId);

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

        #endregion Users

        #region Scheduled Events

        public static async Task<List<ScheduledEvent>> GetAllEventsAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.ScheduledEvents.ToListAsync();
            }
        }

        public static async Task<List<ScheduledEvent>> GetEventsAsync(Expression<Func<ScheduledEvent, bool>> predicate)
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

        public static async Task<List<RiftTempRole>> GetUserTempRolesAsync(ulong userId)
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

        public static async Task<int> GetTempRolesCountAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.TempRoles.CountAsync();
            }
        }

        public static async Task<bool> HasTempRoleAsync(ulong userId, ulong roleId)
        {
            using (var context = new RiftContext())
            {
                return await context.TempRoles.AnyAsync(x => x.UserId == userId && x.RoleId == roleId);
            }
        }

        public static async Task RemoveUserTempRoleAsync(ulong userId, ulong roleId)
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
        
        public static async Task<RiftStreamer> GetStreamer(ulong userId)
        {
            using (var context = new RiftContext())
            {
                return await context.Streamers
                    .Where(x => x.UserId == userId)
                    .FirstOrDefaultAsync();
            }
        }
        
        #endregion Streamers
        
        #region Stored Messages

        public static async Task<RiftMapping> GetMessageMappingByNameAsync(string identifier)
        {
            using (var context = new RiftContext())
            {
                return await context.MessageMappings.FirstOrDefaultAsync(x => 
                    x.Identifier.Equals(identifier, StringComparison.InvariantCultureIgnoreCase));
            }
        }
        
        public static async Task<RiftMessage> GetMessageByIdAsync(int id)
        {
            using (var context = new RiftContext())
            {
                return await context.StoredMessages
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
        }

        public static async Task AddStoredMessage(RiftMessage message)
        {
            using (var context = new RiftContext())
            {
                await context.StoredMessages.AddAsync(message);
                await context.SaveChangesAsync();
            }
        }

        #endregion Stored Messages

        #region Toxicity

        static async Task EnsureToxicityExistsAsync(ulong userId)
        {
            await EnsureUserExistsAsync(userId);

            var dbToxicity = await GetToxicityAsync(userId);

            if (dbToxicity != null)
                return;

            using (var context = new RiftContext())
            {
                var toxicity = new RiftToxicity
                {
                    UserId = userId,
                    LastIncreased = DateTime.MinValue,
                };

                await context.Toxicity.AddAsync(toxicity);
                await context.SaveChangesAsync();
            }
        }

        public static async Task<RiftToxicity> GetToxicityAsync(ulong userId)
        {
            using (var context = new RiftContext())
            {
                return await context.Toxicity.FirstOrDefaultAsync(x => x.UserId == userId);
            }
        }

        public static async Task<bool> HasBlockingToxicityLevelAsync(ulong userId)
        {
            var toxicity = await GetToxicityAsync(userId);

            if (toxicity is null)
                return false;

            return toxicity.Level == 2u;
        }

        public static async Task<RiftToxicity[]> GetToxicityNonZeroAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.Toxicity
                    .Where(x => x.Level > 0u)
                    .ToArrayAsync();
            }
        }

        public static async Task UpdateToxicityPercentAsync(ulong userId, uint percent)
        {
            await EnsureToxicityExistsAsync(userId);

            var oldToxicity = await GetToxicityAsync(userId);

            using (var context = new RiftContext())
            {
                var toxicity = new RiftToxicity
                {
                    UserId = userId,
                    Percent = percent
                };

                context.Entry(toxicity).Property(x => x.Percent).IsModified = true;

                if (percent > oldToxicity.Percent)
                {
                    toxicity.LastIncreased = DateTime.UtcNow;
                    context.Entry(toxicity).Property(x => x.LastIncreased).IsModified = true;
                }
                else if (percent < oldToxicity.Percent)
                {
                    toxicity.LastDecreased = DateTime.UtcNow;
                    context.Entry(toxicity).Property(x => x.LastDecreased).IsModified = true;
                }

                await context.SaveChangesAsync();
            }
        }

        #endregion Toxicity

        #region Moderation Logs

        public static async Task AddModerationLogAsync(ulong targetId, ulong moderatorId, string action, string reason, DateTime createdAt, TimeSpan duration)
        {
            var log = new RiftModerationLog
            {
                TargetId = targetId,
                ModeratorId = moderatorId,
                Action = action,
                Reason = reason,
                CreatedAt = createdAt,
                Duration = duration
            };

            using (var context = new RiftContext())
            {
                await context.AddAsync(log);
                await context.SaveChangesAsync();
            }
        }

        public static async Task<List<RiftModerationLog>> GetModerationLogsForUserAsync(ulong userId)
        {
            using (var context = new RiftContext())
            {
                return await context.ModerationLog
                    .Where(x => x.TargetId == userId)
                    .OrderByDescending(x => x.CreatedAt)
                    .Take(10)
                    .ToListAsync();
            }
        }

        public static async Task<List<RiftModerationLog>> GetLastModerationLogsAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.ModerationLog
                    .OrderByDescending(x => x.CreatedAt)
                    .Take(10)
                    .ToListAsync();
            }
        }

        #endregion Moderation Logs

        #region Settings

        public static async Task<RiftSettings> GetSettingsAsync(int id)
        {
            using (var context = new RiftContext())
            {
                return await context.Settings
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
        }

        public static async Task SetSettingsAsync(int id, string data)
        {
            using (var context = new RiftContext())
            {
                var settings = new RiftSettings
                {
                    Id = id,
                    Data = data
                };

                if (await context.Settings.AnyAsync(x => x.Id == id))
                {
                    context.Entry(settings).Property(x => x.Data).IsModified = true;
                }
                else
                {
                    await context.Settings.AddAsync(settings);
                }

                await context.SaveChangesAsync();
            }
        }

        #endregion Settings

        #region System Timers

        public static async Task<RiftSystemTimer> GetSystemTimerAsync(string name)
        {
            using (var context = new RiftContext())
            {
                return await context.SystemCooldowns.FirstOrDefaultAsync(x =>
                    x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public static async Task UpdateSystemTimerAsync(string name, DateTime lastUpdated)
        {
            var timer = await GetSystemTimerAsync(name);

            if (timer is null)
            {
                RiftBot.Log.Error($"Timer \"{name}\" does not exist!");
                return;
            }

            using (var context = new RiftContext())
            {
                timer.LastInvoked = lastUpdated;
                context.Entry(timer).Property(x => x.LastInvoked).IsModified = true;
                await context.SaveChangesAsync();
            }
        }

        #endregion System Timers

        #region Rewards

        public static async Task AddRewardAsync(RiftReward reward)
        {
            using (var context = new RiftContext())
            {
                await context.Rewards.AddAsync(reward);
                await context.SaveChangesAsync();
            }
        }

        public static async Task<RiftReward> GetRewardAsync(int id)
        {
            using (var context = new RiftContext())
            {
                return await context.Rewards
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
        }

        public static async Task<bool> TryUpdateRewardAsync(RiftReward reward)
        {
            using (var context = new RiftContext())
            {
                var data = await GetRewardAsync(reward.Id);

                if (data is null)
                    return false; // nothing to update

                var entry = context.Entry(data);

                if (!reward.Description.Equals(data.Description))
                    entry.Property(x => x.Description).IsModified = true;

                if (!reward.ItemsData.Equals(data.ItemsData))
                    entry.Property(x => x.ItemsData).IsModified = true;

                if (!reward.RoleData.Equals(data.RoleData))
                    entry.Property(x => x.RoleData).IsModified = true;

                await context.SaveChangesAsync();
                return true;
            }
        }

        #endregion Rewards

        #region Giveaways

        public static async Task<RiftGiveaway> GetGiveawayAsync(string name)
        {
            using (var context = new RiftContext())
            {
                return await context.Giveaways
                    .FirstOrDefaultAsync(x => x.Name.Equals(name));
            }
        }

        public static async Task AddOrUpdateGiveawayAsync(RiftGiveaway giveaway)
        {
            using (var context = new RiftContext())
            {
                var dbGiveaway = await GetGiveawayAsync(giveaway.Name);

                if (dbGiveaway is null)
                {
                    await context.Giveaways.AddAsync(giveaway);
                }
                else
                {
                    var entry = context.Entry(giveaway);

                    if (!dbGiveaway.Description.Equals(giveaway.Description))
                        entry.Property(x => x.Description).IsModified = true;

                    if (!dbGiveaway.WinnersAmount.Equals(giveaway.WinnersAmount))
                        entry.Property(x => x.WinnersAmount).IsModified = true;

                    if (!dbGiveaway.RewardId.Equals(giveaway.RewardId))
                        entry.Property(x => x.RewardId).IsModified = true;

                    if (!dbGiveaway.Duration.Equals(giveaway.Duration))
                        entry.Property(x => x.Duration).IsModified = true;

                    if (!dbGiveaway.CreatedAt.Equals(giveaway.CreatedAt))
                        entry.Property(x => x.CreatedAt).IsModified = true;

                    if (!dbGiveaway.CreatedBy.Equals(giveaway.CreatedBy))
                        entry.Property(x => x.CreatedBy).IsModified = true;
                }

                await context.SaveChangesAsync();
            }
        }

        #endregion Giveaways

        #region Giveaway Logs

        public static async Task AddGiveawayLogAsync(RiftGiveawayLog log)
        {
            using (var context = new RiftContext())
            {
                await context.GiveawayLogs.AddAsync(log);
                await context.SaveChangesAsync();
            }
        }

        public static async Task<RiftGiveawayLog> GetGiveawayLogAsync(int id)
        {
            using (var context = new RiftContext())
            {
                return await context.GiveawayLogs.FirstOrDefaultAsync(x => x.Id == id);
            }
        }

        public static async Task<List<RiftGiveawayLog>> GetStartedGiveawayLogAsync(Expression<Func<RiftGiveawayLog, bool>> predicate)
        {
            using (var context = new RiftContext())
            {
                return await context.GiveawayLogs
                    .Where(predicate)
                    .ToListAsync();
            }
        }

        #endregion Giveaway Logs

        #region Active Giveaways

        public static async Task AddActiveGiveawayAsync(RiftGiveawayActive giveaway)
        {
            using (var context = new RiftContext())
            {
                await context.ActiveGiveaways.AddAsync(giveaway);
                await context.SaveChangesAsync();
            }
        }

        public static async Task<List<RiftGiveawayActive>> GetExpiredActiveGiveawaysAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.ActiveGiveaways
                    .Where(x => x.DueTime < DateTime.UtcNow)
                    .ToListAsync();
            }
        }

        public static async Task<List<RiftGiveawayActive>> GetLinkedActiveGiveawaysAsync(string giveawayName)
        {
            using (var context = new RiftContext())
            {
                return await context.ActiveGiveaways
                    .Where(x => x.GiveawayName.Equals(giveawayName))
                    .ToListAsync();
            }
        }

        public static async Task<RiftGiveawayActive> GetActiveGiveawayAsync(int id)
        {
            using (var context = new RiftContext())
            {
                return await context.ActiveGiveaways
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
        }

        public static async Task<List<RiftGiveawayActive>> GetActiveGiveawaysAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.ActiveGiveaways.ToListAsync();
            }
        }

        #endregion Active Giveaways
    }

    public class DatabaseException : Exception
    {
        public new readonly string Message;

        public DatabaseException(string message) : base(message)
        {
            Message = message;
        }
    }

    public class InventoryData
    {
        public uint Coins { get; set; } = 0u;
        public uint Tokens { get; set; } = 0u;
        public uint Chests { get; set; } = 0u;
        public uint Spheres { get; set; } = 0u;
        public uint Capsules { get; set; } = 0u;
        public uint Tickets { get; set; } = 0u;
        public uint DoubleExps { get; set; } = 0u;
        public uint BotRespects { get; set; } = 0u;
        public uint Rewinds { get; set; } = 0u;
    }
}
