using System;
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

using IonicLib.Extensions;

using Microsoft.EntityFrameworkCore;

using Rift.Services.Role;

namespace Rift.Services
{
    public class DatabaseService
    {
        public delegate void DonateAdded(DonateAddedEventArgs e);

        public event DonateAdded OnDonateAdded;

        static async Task<bool> EnsureUserExistsAsync(ulong userId)
        {
            using (var context = new RiftContext())
            {
                if (await context.Users.AnyAsync(x => x.UserId == userId))
                    return true;

                try
                {
                    var newUser = new RiftUser()
                    {
                        UserId = userId,
                    };

                    await context.Users.AddAsync(newUser);
                    await context.SaveChangesAsync();

                    return true;
                }
                catch
                {
                    Console.WriteLine($"Failed to create db user {userId}.");
                    return false;
                }
            }
        }

        #region Getters

        public async Task<int> GetUserCountAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.Users.CountAsync();
            }
        }

        public async Task<UserProfile> GetUserProfileAsync(ulong userId)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

            using (var context = new RiftContext())
            {
                return await context.Users
                                    .Join(context.Timestamps, user => user.UserId, time => time.UserId, (riftUser, riftTimestamp) => new { User = riftUser, Timestamps = riftTimestamp })
                                    .Where(x => x.User.UserId == userId && x.Timestamps.UserId == userId)
                                    .Select(x => new UserProfile
                                    {
                                        UserId = x.User.UserId,
                                        Experience = x.User.Experience,
                                        Level = x.User.Level,
                                        TotalDonate = x.User.Donate,
                                        DoubleExpTimestamp = x.Timestamps.DoubleExpTimestamp,
                                        BotRespectTimestamp = x.Timestamps.BotRespectTimestamp,
                                    })
                                    .FirstOrDefaultAsync();
            }
        }

        public async Task<UserInventory> GetUserInventoryAsync(ulong userId)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

            using (var context = new RiftContext())
            {
                return await context.Inventory
                                    .Join(context.Statistics, inv => inv.UserId, stat => stat.UserId, (inventory, statistics) =>
                                              new { RiftInventory = inventory, RiftStatistics = statistics})
                                    .Where(x => x.RiftInventory.UserId == userId && x.RiftStatistics.UserId == userId)
                                    .Select(x => new UserInventory
                                    {
                                        UserId = x.RiftInventory.UserId,
                                        Coins = x.RiftInventory.Coins,
                                        Tokens = x.RiftInventory.Tokens,
                                        Chests = x.RiftInventory.Chests,
                                        Capsules = x.RiftInventory.Capsules,
                                        Spheres = x.RiftInventory.Spheres,
                                        PowerupsDoubleExperience = x.RiftInventory.PowerupsDoubleExp,
                                        PowerupsBotRespect = x.RiftInventory.PowerupsBotRespect,
                                        CoinsEarnedTotal = x.RiftStatistics.CoinsEarnedTotal,
                                        CoinsSpentTotal = x.RiftStatistics.CoinsSpentTotal,
                                        UsualTickets = x.RiftInventory.UsualTickets,
                                        RareTickets = x.RiftInventory.RareTickets,
                                    })
                                    .FirstOrDefaultAsync();
            }
        }

        public async Task<UserChests> GetUserChestsAsync(ulong userId)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

            using (var context = new RiftContext())
            {
                return await context.Inventory
                                    .Where(x => x.UserId == userId)
                                    .Select(x => new UserChests
                                    {
                                        UserId = x.UserId,
                                        Chests = x.Chests,
                                    })
                                    .FirstOrDefaultAsync();
            }
        }

        async Task<UserDonate> GetUserDonateAsync(ulong userId)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

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

        public async Task<RiftLolData> GetUserLolDataAsync(ulong userId)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

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
                                    .FirstAsync();
            }
        }

        public async Task<RiftLolData[]> GetExistingSummonersAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.LolData
                                    .Where(x => !String.IsNullOrWhiteSpace(x.PlayerUUID))
                                    .Select(x => new RiftLolData
                                    {
                                        UserId = x.UserId,
                                        SummonerRegion = x.SummonerRegion,
                                        PlayerUUID = x.PlayerUUID,
                                        AccountId = x.AccountId,
                                        SummonerId = x.SummonerId,
                                        SummonerName = x.SummonerName,
                                    })
                                    .ToArrayAsync();
            }
        }

        public async Task<List<UserDonate>> GetAllDonatesAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.Users
                                    .Where(x => x.Donate > 0M)
                                    .Select(x => new UserDonate
                                    {
                                        UserId = x.UserId,
                                        Donate = x.Donate,
                                    })
                                    .OrderByDescending(x => x.Donate)
                                    .ToListAsync();
            }
        }

        public async Task<UserDonate> GetTopDonateAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.Users
                                    .Where(x => x.Donate > 0M)
                                    .Select(x => new UserDonate
                                    {
                                        UserId = x.UserId,
                                        Donate = x.Donate,
                                    })
                                    .OrderByDescending(x => x.Donate)
                                    .FirstOrDefaultAsync();
            }
        }

        public async Task<UserLevel> GetUserLevelAsync(ulong userId)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

            using (var context = new RiftContext())
            {
                return await context.Users
                                    .Join(context.Timestamps, user => user.UserId, time => time.UserId, (user, timestamps) => new {User = user, Timestamps = timestamps})
                                    .Where(x => x.User.UserId == userId && x.Timestamps.UserId == userId)
                                    .Select(x => new UserLevel
                                    {
                                        UserId = x.User.UserId,
                                        Level = x.User.Level,
                                        Experience = x.User.Experience,
                                        DoubleExpTimestamp = x.Timestamps.DoubleExpTimestamp
                                    })
                                    .FirstAsync();
            }
        }

        public async Task<RiftAchievements> GetUserAchievementsAsync(ulong userId)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

            using (var context = new RiftContext())
            {
                return await context.Achievements
                                    .Where(x => x.UserId == userId)
                                    .Select(x => new RiftAchievements
                                    {
                                        UserId = x.UserId,
                                        Write100Messages = x.Write100Messages,
                                        Write1000Messages = x.Write1000Messages,
                                        Reach10Level = x.Reach10Level,
                                        Reach30Level = x.Reach30Level,
                                        Brag100Times = x.Brag100Times,
                                        Attack200Times = x.Attack200Times,
                                        OpenSphere = x.OpenSphere,
                                        GiftSphere = x.GiftSphere,
                                        Purchase200Items = x.Purchase200Items,
                                        Open100Chests = x.Open100Chests,
                                        Send100Gifts = x.Send100Gifts,
                                        IsDonator = x.IsDonator,
                                        HasDonatedRole = x.HasDonatedRole,
                                        GiftToBotKeeper = x.GiftToBotKeeper,
                                        GiftToModerator = x.GiftToModerator,
                                        AttackWise = x.AttackWise,
                                    })
                                    .FirstAsync();
            }
        }

        public async Task<UserStatistics> GetUserStatisticsAsync(ulong userId)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

            using (var context = new RiftContext())
            {
                return await context.Statistics
                                    .Join(context.Achievements, s => s.UserId, a => a.UserId, (statistics, achievements) => new { Statistics = statistics, Achievements = achievements})
                                    .Where(x => x.Statistics.UserId == userId && x.Achievements.UserId == userId)
                                    .Select(x => new UserStatistics
                                    {
                                        UserId = x.Statistics.UserId,
                                        CoinsEarnedTotal = x.Statistics.CoinsEarnedTotal,
                                        TokensEarnedTotal = x.Statistics.TokensEarnedTotal,
                                        ChestsEarnedTotal = x.Statistics.ChestsEarnedTotal,
                                        SphereEarnedTotal = x.Statistics.SphereEarnedTotal,
                                        CapsuleEarnedTotal = x.Statistics.CapsuleEarnedTotal,
                                        ChestsOpenedTotal = x.Statistics.ChestsOpenedTotal,
                                        SphereOpenedTotal = x.Statistics.SphereEarnedTotal,
                                        CapsuleOpenedTotal = x.Statistics.CapsuleEarnedTotal,
                                        AttacksDone = x.Statistics.AttacksDone,
                                        AttacksReceived = x.Statistics.AttacksReceived,
                                        CoinsSpentTotal = x.Statistics.CoinsSpentTotal,
                                        TokensSpentTotal = x.Statistics.TokensSpentTotal,
                                        GiftsSent = x.Statistics.GiftsSent,
                                        GiftsReceived = x.Statistics.GiftsReceived,
                                        MessagesSentTotal = x.Statistics.MessagesSentTotal,
                                        BragTotal = x.Statistics.BragTotal,
                                        PurchasedItemsTotal = x.Statistics.PurchasedItemsTotal,
                                        AchievementsCount = Convert.ToUInt32(x.Achievements.Write100Messages)
                                                            + Convert.ToUInt32(x.Achievements.Write1000Messages)
                                                            + Convert.ToUInt32(x.Achievements.Reach10Level)
                                                            + Convert.ToUInt32(x.Achievements.Reach30Level)
                                                            + Convert.ToUInt32(x.Achievements.Brag100Times)
                                                            + Convert.ToUInt32(x.Achievements.Attack200Times)
                                                            + Convert.ToUInt32(x.Achievements.OpenSphere)
                                                            + Convert.ToUInt32(x.Achievements.GiftSphere)
                                                            + Convert.ToUInt32(x.Achievements.Purchase200Items)
                                                            + Convert.ToUInt32(x.Achievements.Open100Chests)
                                                            + Convert.ToUInt32(x.Achievements.Send100Gifts)
                                                            + Convert.ToUInt32(x.Achievements.IsDonator)
                                                            + Convert.ToUInt32(x.Achievements.HasDonatedRole)
                                                            + Convert.ToUInt32(x.Achievements.GiftToBotKeeper)
                                                            + Convert.ToUInt32(x.Achievements.GiftToModerator)
                                                            + Convert.ToUInt32(x.Achievements.AttackWise),
                                    })
                                    .FirstAsync();
            }
        }

        public async Task<UserDoubleExpTimestamp> GetUserDoubleExpTimestampAsync(ulong userId)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

            using (var context = new RiftContext())
            {
                return await context.Timestamps
                                    .Where(x => x.UserId == userId)
                                    .Select(x => new UserDoubleExpTimestamp
                                    {
                                        UserId = x.UserId,
                                        DoubleExpTimestamp = x.DoubleExpTimestamp,
                                    })
                                    .FirstAsync();
            }
        }

        public async Task<UserLastStoreTimestamp> GetUserLastStoreTimestampAsync(ulong userId)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

            using (var context = new RiftContext())
            {
                return await context.Timestamps
                                    .Where(x => x.UserId == userId)
                                    .Select(x => new UserLastStoreTimestamp
                                    {
                                        UserId = x.UserId,
                                        LastStoreTimestamp = x.LastStoreTimestamp,
                                    })
                                    .FirstAsync();
            }
        }

        public async Task<UserLastAttackTimestamp> GetUserLastAttackTimestampAsync(ulong userId)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

            using (var context = new RiftContext())
            {
                return await context.Timestamps
                                    .Where(x => x.UserId == userId)
                                    .Select(x => new UserLastAttackTimestamp
                                    {
                                        UserId = x.UserId,
                                        LastAttackTimestamp = x.LastAttackTimestamp,
                                    })
                                    .FirstAsync();
            }
        }

        public async Task<UserLastBeingAttackedTimestamp> GetUserLastBeingAttackedTimestampAsync(ulong userId)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

            using (var context = new RiftContext())
            {
                return await context.Timestamps
                                    .Where(x => x.UserId == userId)
                                    .Select(x => new UserLastBeingAttackedTimestamp
                                    {
                                        UserId = x.UserId,
                                        LastBeingAttackedTimestamp = x.LastBeingAttackedTimestamp,
                                    })
                                    .FirstAsync();
            }
        }

        public async Task<UserLastDailyChestTimestamp> GetUserLastDailyChestTimestampAsync(ulong userId)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

            using (var context = new RiftContext())
            {
                return await context.Timestamps
                                    .Where(x => x.UserId == userId)
                                    .Select(x => new UserLastDailyChestTimestamp
                                    {
                                        UserId = x.UserId,
                                        LastDailyChestTimestamp = x.LastDailyChestTimestamp,
                                    })
                                    .FirstOrDefaultAsync();
            }
        }

        public async Task<UserLastBragTimestamp> GetUserLastBragTimestampAsync(ulong userId)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

            using (var context = new RiftContext())
            {
                return await context.Timestamps
                                    .Where(x => x.UserId == userId)
                                    .Select(x => new UserLastBragTimestamp
                                    {
                                        UserId = x.UserId,
                                        LastBragTimestamp = x.LastBragTimestamp,
                                    })
                                    .FirstAsync();
            }
        }

        public async Task<long> GetTotalExpAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.Users.SumAsync(x => x.Experience);
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

        public async Task<bool> HasLevelAsync(ulong userId, uint level)
        {
            return (await GetUserLevelAsync(userId)).Level >= level;
        }

        public async Task<bool> HasLolDataAsync(ulong userId)
        {
            return !String.IsNullOrWhiteSpace((await GetUserLolDataAsync(userId)).PlayerUUID);
        }

        public async Task<bool> IsTakenAsync(string region, string playerUUID)
        {
            using (var context = new RiftContext())
            {
                return await context.LolData.AnyAsync(x => x.PlayerUUID == playerUUID
                                                         && x.SummonerRegion == region);
            }
        }

        public async Task<UserLastGiftTimestamp> GetUserLastGiftTimestampAsync(ulong userId)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

            using (var context = new RiftContext())
            {
                return await context.Timestamps
                                    .Where(x => x.UserId == userId)
                                    .Select(x => new UserLastGiftTimestamp
                                    {
                                        UserId = x.UserId,
                                        LastGiftTimestamp = x.LastGiftTimestamp,
                                    })
                                    .FirstAsync();
            }
        }

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

        public async Task<UserBotRespectTimestamp[]> GetBotRespectedUsersAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.Timestamps
                                    .Select(x => new UserBotRespectTimestamp
                                    {
                                        UserId = x.UserId,
                                        BotRespectTimestamp = x.BotRespectTimestamp
                                    })
                                    .Where(x => x.BotRespectTimestamp > Helper.CurrentUnixTimestamp)
                                    .ToArrayAsync();
            }
        }

        public async Task<UserLastLolAccountUpdateTimestamp[]> GetTenUsersForUpdateAsync()
        {
            using (var context = new RiftContext())
            {
                var list = await context.Timestamps
                                        .Select(x => new UserLastLolAccountUpdateTimestamp
                                        {
                                            UserId = x.UserId,
                                            LastUpdateTimestamp = x.LastLolAccountUpdateTimestamp,
                                        })
                                        .OrderBy(x => x.LastUpdateTimestamp)
                                        .Where(x => !String.IsNullOrWhiteSpace(x.PlayerUuid))
                                        .ToArrayAsync();

                return list.Take(10).ToArray();
            }
        }

        #endregion Getters

        #region Setters

        public async Task SetLolDataAsync(ulong userId, string region, string playerUuid, string accountId, string summonerId, string summonerName)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

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
                var entry = context.Attach(lolData);

                if (dbSummoner.SummonerRegion != region)
                    entry.Property(x => x.SummonerRegion).IsModified = true;

                if (dbSummoner.PlayerUUID != playerUuid)
                    entry.Property(x => x.PlayerUUID).IsModified = true;

                if (dbSummoner.SummonerId != summonerId)
                    entry.Property(x => x.SummonerId).IsModified = true;

                if (dbSummoner.SummonerName != summonerName)
                    entry.Property(x => x.SummonerName).IsModified = true;

                await context.SaveChangesAsync();
            }
        }
        
        public async Task ClearLolDataAsync(ulong userId)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

            var lolData = new RiftLolData
            {
                UserId = userId,
                SummonerRegion = "",
                AccountId = "",
                PlayerUUID = "",
                SummonerId = "",
                SummonerName = "",
            };

            using (var context = new RiftContext())
            {
                context.Attach(lolData).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }

        public async Task SetLastStoreTimestampAsync(ulong userId, ulong timestamp)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

            var ts = new UserLastStoreTimestamp
            {
                UserId = userId,
                LastStoreTimestamp = timestamp,
            };

            using (var context = new RiftContext())
            {
                context.Attach(ts).Property(x => x.LastStoreTimestamp).IsModified = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task SetLastAttackTimestampAsync(ulong userId, ulong timestamp)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

            var ts = new UserLastAttackTimestamp
            {
                UserId = userId,
                LastAttackTimestamp = timestamp,
            };

            using (var context = new RiftContext())
            {
                context.Attach(ts).Property(x => x.LastAttackTimestamp).IsModified = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task SetLastBeingAttackedTimestampAsync(ulong userId, ulong timestamp)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

            var ts = new UserLastBeingAttackedTimestamp
            {
                UserId = userId,
                LastBeingAttackedTimestamp = timestamp,
            };

            using (var context = new RiftContext())
            {
                context.Attach(ts).Property(x => x.LastBeingAttackedTimestamp).IsModified = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task SetLastDailyChestTimestampAsync(ulong userId, ulong timestamp)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

            var ts = new UserLastDailyChestTimestamp
            {
                UserId = userId,
                LastDailyChestTimestamp = timestamp,
            };

            using (var context = new RiftContext())
            {
                context.Attach(ts).Property(x => x.LastDailyChestTimestamp).IsModified = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task SetDoubleExpTimestampAsync(ulong userId, ulong timestamp)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

            var ts = new UserDoubleExpTimestamp
            {
                UserId = userId,
                DoubleExpTimestamp = timestamp,
            };

            using (var context = new RiftContext())
            {
                context.Attach(ts).Property(x => x.DoubleExpTimestamp).IsModified = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task SetBotRespeсtTimestampAsync(ulong userId, ulong timestamp)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

            var ts = new UserBotRespectTimestamp
            {
                UserId = userId,
                BotRespectTimestamp = timestamp,
            };

            using (var context = new RiftContext())
            {
                context.Attach(ts).Property(x => x.BotRespectTimestamp).IsModified = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task SetLevelAsync(ulong userId, uint level)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

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

        public async Task SetExpAsync(ulong userId, uint exp)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

            var user = new RiftUser
            {
                UserId = userId,
                Experience = exp
            };

            using (var context = new RiftContext())
            {
                context.Attach(user).Property(x => x.Experience).IsModified = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task SetLastBragTimestamp(ulong userId, ulong timestamp)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

            var ts = new UserLastBragTimestamp
            {
                UserId = userId,
                LastBragTimestamp = timestamp,
            };

            using (var context = new RiftContext())
            {
                context.Attach(ts).Property(x => x.LastBragTimestamp).IsModified = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task SetLastGiftTimestamp(ulong userId, ulong timestamp)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

            var ts = new UserLastGiftTimestamp
            {
                UserId = userId,
                LastGiftTimestamp = timestamp,
            };

            using (var context = new RiftContext())
            {
                context.Attach(ts).Property(x => x.LastGiftTimestamp).IsModified = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task SetLastLolAccountUpdateTimestamp(ulong userId, ulong timestamp)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

            var ts = new UserLastLolAccountUpdateTimestamp
            {
                UserId = userId,
                LastUpdateTimestamp = timestamp,
            };

            using (var context = new RiftContext())
            {
                context.Attach(ts).Property(x => x.LastUpdateTimestamp).IsModified = true;
                await context.SaveChangesAsync();
            }
        }

        #endregion Setters

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
                throw new InvalidOperationException();

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
                throw new InvalidOperationException();

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

                    Console.WriteLine($"Modified {userId}'s donate ({donateBefore} => {user.Donate})");
                }
            }
        }

        #region Pending Users

        public async Task AddPendingUserAsync(RiftPendingUser pendingUser)
        {
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
                                        ExpirationTimestamp = x.ExpirationTimestamp,
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
                                        ExpirationTimestamp = x.ExpirationTimestamp
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

        public async Task RemovePendingUsersAsync(List<RiftPendingUser> pendingUsersList)
        {
            using (var context = new RiftContext())
            {
                context.PendingUsers.RemoveRange(pendingUsersList);
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
                                    .Where(x => x.ExpirationTimestamp > Helper.CurrentUnixTimestamp)
                                    .ToListAsync();
            }
        }

        #endregion Pending Users

        #region Temp Roles

        public async Task AddTempRoleAsync(RiftTempRole role)
        {
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

        public async Task AddInventoryAsync(ulong userId, uint coins = 0u, uint tokens = 0u, uint chests = 0u,
                                            uint capsules = 0u, uint spheres = 0u, uint doubleExps = 0u,
                                            uint respects = 0u, uint usualTickets = 0u, uint rareTickets = 0u)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

            var dbInventory = await GetUserInventoryAsync(userId);
            var dbStatistics = await GetUserStatisticsAsync(userId);

            var inventory = new RiftInventory
            {
                UserId = userId
            };

            var statistics = new RiftStatistics
            {
                UserId = userId
            };

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

                    Console.WriteLine($"Modified {userId}'s coin(s): ({coinsBefore} => {inventory.Coins})");

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

                    Console.WriteLine($"Modified {userId}'s token(s): ({tokensBefore} => {inventory.Tokens})");

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

                    Console.WriteLine($"Modified {userId}'s chest(s): ({chestsBefore} => {inventory.Chests})");

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

                    Console.WriteLine($"Modified {userId}'s capsule(s): ({capsulesBefore} => {inventory.Capsules})");

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

                    Console.WriteLine($"Modified {userId}'s sphere(s): ({spheresBefore} => {inventory.Spheres})");

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

                    Console.WriteLine($"Modified {userId}'s doubleExp(s): ({doubleExpsBefore} => {inventory.PowerupsDoubleExp})");
                }

                if (respects > uint.MinValue)
                {
                    uint respectsBefore = dbInventory.PowerupsBotRespect;

                    if (uint.MaxValue - respectsBefore < respects)
                        inventory.PowerupsBotRespect = uint.MaxValue;
                    else
                        inventory.PowerupsBotRespect = respectsBefore + respects;

                    inventoryEntry.Property(x => x.PowerupsBotRespect).IsModified = true;

                    Console.WriteLine($"Modified {userId}'s respect(s): ({respectsBefore} => {inventory.PowerupsBotRespect})");
                }

                if (usualTickets > uint.MinValue)
                {
                    uint usualTicketsBefore = dbInventory.UsualTickets;

                    if (uint.MaxValue - usualTicketsBefore < usualTickets)
                        inventory.UsualTickets = uint.MaxValue;
                    else
                        inventory.UsualTickets = usualTicketsBefore + usualTickets;

                    inventoryEntry.Property(x => x.UsualTickets).IsModified = true;

                    Console.WriteLine($"Modified {userId}'s usual ticket(s): ({usualTicketsBefore} => {inventory.UsualTickets})");
                }

                if (rareTickets > uint.MinValue)
                {
                    uint rareTicketsBefore = dbInventory.RareTickets;

                    if (uint.MaxValue - rareTicketsBefore < rareTickets)
                        inventory.RareTickets = uint.MaxValue;
                    else
                        inventory.RareTickets = rareTicketsBefore + rareTickets;

                    inventoryEntry.Property(x => x.RareTickets).IsModified = true;

                    Console.WriteLine($"Modified {userId}'s rare ticket(s): ({rareTicketsBefore} => {inventory.RareTickets})");
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task RemoveInventoryAsync(ulong userId, uint coins = 0u, uint tokens = 0u, uint chests = 0u,
                                               uint capsules = 0u, uint spheres = 0u, uint doubleExps = 0u,
                                               uint respects = 0u, uint usualTickets = 0u, uint rareTickets = 0u)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

            var dbInventory = await GetUserInventoryAsync(userId);
            var dbStatistics = await GetUserStatisticsAsync(userId);

            var inventory = new RiftInventory
            {
                UserId = userId
            };

            var statistics = new RiftStatistics
            {
                UserId = userId
            };

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

                    Console.WriteLine($"Modified {userId}'s coin(s): ({coinsBefore} => {inventory.Coins})");

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

                    Console.WriteLine($"Modified {userId}'s token(s): ({tokensBefore} => {inventory.Tokens})");

                    inventoryEntry.Property(x => x.Tokens).IsModified = true;
                    statisticsEntry.Property(x => x.TokensSpentTotal).IsModified = true;
                }

                uint chestsBefore = dbInventory.Chests;
                chests = Math.Min(chests, chestsBefore);
                if (chests > uint.MinValue)
                {
                    inventory.Chests = chestsBefore - chests;
                    Console.WriteLine($"Modified {userId}'s chest(s): ({chestsBefore} => {inventory.Chests})");
                    inventoryEntry.Property(x => x.Chests).IsModified = true;
                }

                uint capsulesBefore = dbInventory.Capsules;
                capsules = Math.Min(capsules, capsulesBefore);
                if (capsules > uint.MinValue)
                {
                    inventory.Capsules = capsulesBefore - capsules;
                    Console.WriteLine($"Modified {userId}'s capsule(s): ({capsulesBefore} => {inventory.Capsules})");
                    inventoryEntry.Property(x => x.Capsules).IsModified = true;
                }

                uint spheresBefore = dbInventory.Spheres;
                spheres = Math.Min(spheres, spheresBefore);
                if (spheres > uint.MinValue)
                {
                    inventory.Spheres = spheresBefore - spheres;
                    Console.WriteLine($"Modified {userId}'s sphere(s): ({spheresBefore} => {inventory.Spheres})");
                    inventoryEntry.Property(x => x.Spheres).IsModified = true;
                }

                uint doubleExpsBefore = dbInventory.PowerupsDoubleExperience;
                doubleExps = Math.Min(doubleExps, doubleExpsBefore);
                if (doubleExps > uint.MinValue)
                {
                    inventory.PowerupsDoubleExp = doubleExpsBefore - doubleExps;
                    inventoryEntry.Property(x => x.PowerupsDoubleExp).IsModified = true;
                    Console.WriteLine($"Modified {userId}'s doubleExp(s): ({doubleExpsBefore} => {inventory.PowerupsDoubleExp})");
                }

                uint respectsBefore = dbInventory.PowerupsBotRespect;
                respects = Math.Min(respects, respectsBefore);
                if (respects > uint.MinValue)
                {
                    inventory.PowerupsBotRespect = respectsBefore - respects;
                    inventoryEntry.Property(x => x.PowerupsBotRespect).IsModified = true;
                    Console.WriteLine($"Modified {userId}'s respect(s): ({respectsBefore} => {inventory.PowerupsBotRespect})");
                }

                uint usualTicketsBefore = dbInventory.UsualTickets;
                usualTickets = Math.Min(usualTickets, usualTicketsBefore);
                if (usualTickets > uint.MinValue)
                {
                    inventory.UsualTickets = usualTicketsBefore - usualTickets;
                    inventoryEntry.Property(x => x.UsualTickets).IsModified = true;
                    Console.WriteLine($"Modified {userId}'s usual ticket(s): ({usualTicketsBefore} => {inventory.UsualTickets})");
                }

                uint rareTicketsBefore = dbInventory.RareTickets;
                rareTickets = Math.Min(rareTickets, rareTicketsBefore);
                if (rareTickets > uint.MinValue)
                {
                    inventory.RareTickets = rareTicketsBefore - rareTickets;
                    inventoryEntry.Property(x => x.RareTickets).IsModified = true;
                    Console.WriteLine($"Modified {userId}'s rare ticket(s): ({rareTicketsBefore} => {inventory.RareTickets})");
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task AddExperienceAsync(ulong userId, uint exp = 0u)
        {
            if (exp == uint.MinValue)
                return;

            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

            var dbUser = new RiftUser
            {
                UserId = userId
            };

            var profile = await GetUserProfileAsync(userId);

            using (var context = new RiftContext())
            {
                var entry = context.Attach(dbUser);

                if (Helper.CurrentUnixTimestamp < profile.DoubleExpTimestamp)
                    exp *= 2;

                uint expBefore = profile.Experience;

                if (uint.MaxValue - expBefore < exp)
                    dbUser.Experience = uint.MaxValue;
                else
                    dbUser.Experience = expBefore + exp;

                if (exp > 2)
                    Console.WriteLine($"Modified {userId}'s exp(s): ({expBefore} => {dbUser.Experience})");

                entry.Property(x => x.Experience).IsModified = true;

                await context.SaveChangesAsync();
            }
        }

        public async Task AddStatisticsAsync(ulong userId, ulong giftsSent = 0ul, ulong giftsReceived = 0ul,
                                             ulong bragTotal = 0ul, ulong chestsOpenedTotal = 0ul,
                                             ulong sphereOpenedTotal = 0ul,
                                             ulong capsuleOpenedTotal = 0ul, ulong attacksDone = 0ul,
                                             ulong attacksReceived = 0ul,
                                             ulong messagesSentTotal = 0ul, ulong purchasedItemsTotal = 0ul)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

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

        public async Task UpdateAchievementAsync(ulong userId, bool write100Messages = false,
                                                 bool write1000Messages = false, bool reach10Level = false,
                                                 bool reach30Level = false,
                                                 bool brag100Times = false, bool attack200Times = false,
                                                 bool openSphere = false, bool giftSphere = false,
                                                 bool purchase200Items = false, bool open100Chests = false,
                                                 bool send100Gifts = false, bool isDonator = false,
                                                 bool hasDonateRole = false, bool giftToBotKeeper = false,
                                                 bool giftToModerator = false, bool attackWise = false)
        {
            if (!await EnsureUserExistsAsync(userId))
                throw new InvalidOperationException();

            var achievements = await GetUserAchievementsAsync(userId);

            var dbAchievements = new RiftAchievements
            {
                UserId = userId
            };

            using (var context = new RiftContext())
            {
                var entry = context.Attach(dbAchievements);

                if (write100Messages && !achievements.Write100Messages)
                {
                    Console.WriteLine($"User {userId} wrote more than 100 messages");

                    dbAchievements.Write100Messages = true;
                    entry.Property(x => x.Write100Messages).IsModified = true;
                }

                if (write1000Messages && !achievements.Write1000Messages)
                {
                    Console.WriteLine($"User {userId} wrote more than 1000 messages");

                    dbAchievements.Write1000Messages = true;
                    entry.Property(x => x.Write1000Messages).IsModified = true;
                }

                if (reach10Level && !achievements.Reach10Level)
                {
                    Console.WriteLine($"User {userId} reached 10 level");

                    dbAchievements.Reach10Level = true;
                    entry.Property(x => x.Reach10Level).IsModified = true;
                }

                if (reach30Level && !achievements.Reach30Level)
                {
                    Console.WriteLine($"User {userId} reached 30 level");

                    dbAchievements.Reach30Level = true;
                    entry.Property(x => x.Reach30Level).IsModified = true;
                }

                if (brag100Times && !achievements.Brag100Times)
                {
                    Console.WriteLine($"User {userId} bragged 50 times");

                    dbAchievements.Brag100Times = true;
                    entry.Property(x => x.Brag100Times).IsModified = true;
                }

                if (attack200Times && !achievements.Attack200Times)
                {
                    Console.WriteLine($"User {userId} did 50 attacks");

                    dbAchievements.Attack200Times = true;
                    entry.Property(x => x.Attack200Times).IsModified = true;
                }

                if (openSphere && !achievements.OpenSphere)
                {
                    Console.WriteLine($"User {userId} opened sphere for the first time");

                    dbAchievements.OpenSphere = true;
                    entry.Property(x => x.OpenSphere).IsModified = true;
                }

                if (giftSphere && !achievements.GiftSphere)
                {
                    Console.WriteLine($"User {userId} gifted sphere for the first time");

                    dbAchievements.GiftSphere = true;
                    entry.Property(x => x.GiftSphere).IsModified = true;
                }

                if (purchase200Items && !achievements.Purchase200Items)
                {
                    Console.WriteLine($"User {userId} purchased 50 items");

                    dbAchievements.Purchase200Items = true;
                    entry.Property(x => x.Purchase200Items).IsModified = true;
                }

                if (open100Chests && !achievements.Open100Chests)
                {
                    Console.WriteLine($"User {userId} opened 100 chests");

                    dbAchievements.Open100Chests = true;
                    entry.Property(x => x.Open100Chests).IsModified = true;
                }

                if (send100Gifts && !achievements.Send100Gifts)
                {
                    Console.WriteLine($"User {userId} sent 50 gifts");

                    dbAchievements.Send100Gifts = true;
                    entry.Property(x => x.Send100Gifts).IsModified = true;
                }

                if (isDonator && !achievements.IsDonator)
                {
                    Console.WriteLine($"User {userId} first time donated");

                    dbAchievements.IsDonator = true;
                    entry.Property(x => x.IsDonator).IsModified = true;
                }

                if (hasDonateRole && !achievements.HasDonatedRole)
                {
                    Console.WriteLine($"User {userId} has donate role");

                    dbAchievements.HasDonatedRole = true;
                    entry.Property(x => x.HasDonatedRole).IsModified = true;
                }

                if (giftToBotKeeper && !achievements.GiftToBotKeeper)
                {
                    Console.WriteLine($"User {userId} made gift to bot keeper");

                    dbAchievements.GiftToBotKeeper = true;
                    entry.Property(x => x.GiftToBotKeeper).IsModified = true;
                }

                if (giftToModerator && !achievements.GiftToModerator)
                {
                    dbAchievements.GiftToModerator = true;
                    entry.Property(x => x.GiftToModerator).IsModified = true;
                }

                if (attackWise && !achievements.AttackWise)
                {
                    dbAchievements.AttackWise = true;
                    entry.Property(x => x.AttackWise).IsModified = true;
                }

                if (context.Entry(entry).State != EntityState.Unchanged)
                    await context.SaveChangesAsync();
            }
        }
    }
}
