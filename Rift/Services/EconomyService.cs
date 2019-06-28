using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Rift.Data;
using Rift.Events;
using Rift.Services.Message;
using Rift.Services.Reward;
using Rift.Services.Store;
using Rift.Util;

using Discord;
using IonicLib;
using Microsoft.EntityFrameworkCore;
using Rift.Database;
using Settings = Rift.Configuration.Settings;

namespace Rift.Services
{
    public class EconomyService
    {
        public static event EventHandler<ChestsOpenedEventArgs> ChestsOpened;
        public static event EventHandler<ActivatedBotRespectsEventArgs> ActivatedBotRespects;
        public static event EventHandler<OpenedSphereEventArgs> OpenedSphere;

        static Timer ratingUpdateTimer;
        static Timer ActiveUsersTimer;
        static Timer RichUsersTimer;
        static readonly TimeSpan ratingTimerCooldown = TimeSpan.FromHours(1);

        static SemaphoreSlim chestMutex = new SemaphoreSlim(1);
        static SemaphoreSlim capsuleMutex = new SemaphoreSlim(1);
        static SemaphoreSlim sphereMutex = new SemaphoreSlim(1);
        
        public void Init()
        {
            ratingUpdateTimer = new Timer(async delegate { await UpdateRatingAsync(); }, null, TimeSpan.FromMinutes(5), ratingTimerCooldown);
            InitActiveUsersTimer();
            InitRichUsersTimer();
        }

        public static List<ulong> SortedRating { get; private set; } = null;

        static void InitActiveUsersTimer()
        {
            var today = DateTime.Today.AddHours(16);

            if (DateTime.UtcNow > today)
                today = today.AddDays(1);

            ActiveUsersTimer = new Timer(async delegate { await ShowActiveUsersAsync(); }, null,
                today - DateTime.UtcNow, TimeSpan.FromDays(1));
        }

        void InitRichUsersTimer()
        {
            var today = DateTime.Today.AddHours(18);

            if (DateTime.UtcNow > today)
                today = today.AddDays(1);

            RichUsersTimer = new Timer(async delegate { await ShowRichUsersAsync(); }, null,
                today - DateTime.UtcNow, TimeSpan.FromDays(1));
        }

        public static async Task ShowActiveUsersAsync()
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Comms, out var commsChannel))
                return;

            var topTen = await DB.Users.GetTopTenByExpAsync(x => 
                !(IonicClient.GetGuildUserById(Settings.App.MainGuildId, x.UserId) is null));

            if (topTen.Length == 0)
                return;

            var msg = await RiftBot.GetMessageAsync("economy-activeusers", new FormatData
            {
                Economy = new EconomyData
                {
                    Top10Exp = topTen
                }
            });
            await commsChannel.SendIonicMessageAsync(msg);
        }

        public static async Task ShowRichUsersAsync()
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Comms, out var commsChannel))
                return;

            var topTen = await DB.Users.GetTopTenByCoinsAsync(x => 
                !(IonicClient.GetGuildUserById(Settings.App.MainGuildId, x.UserId) is null));

            if (topTen.Length == 0)
                return;

            var msg = await RiftBot.GetMessageAsync("economy-richusers", new FormatData
            {
                Economy = new EconomyData
                {
                    Top10Coins = topTen
                }
            });
            await commsChannel.SendIonicMessageAsync(msg);
        }

        public async Task ProcessMessageAsync(IUserMessage message)
        {
            await AddExpAsync(message.Author.Id, 1u).ConfigureAwait(false);
        }

        static async Task AddExpAsync(ulong userId, uint exp)
        {
            await DB.Users.AddExperienceAsync(userId, exp)
                .ContinueWith(async _ => await CheckLevelUpAsync(userId));
        }

        static async Task CheckLevelUpAsync(ulong userId)
        {
            var dbUser = await DB.Users.GetAsync(userId);

            if (dbUser.Experience != uint.MaxValue)
            {
                var newLevel = dbUser.Level + 1u;

                while (dbUser.Experience >= GetExpForLevel(newLevel))
                {
                    newLevel++;
                }

                newLevel--;

                if (newLevel > dbUser.Level)
                {
                    await DB.Users.SetLevelAsync(dbUser.UserId, newLevel);

                    RiftBot.Log.Info($"{dbUser.UserId.ToString()} just leveled up: {dbUser.Level.ToString()} => {newLevel.ToString()}");

                    if (newLevel == 1u)
                        return; //no rewards on level 1

                    await GiveRewardsForLevelAsync(dbUser.UserId, dbUser.Level, newLevel);
                }
            }
        }

        static async Task GiveRewardsForLevelAsync(ulong userId, uint fromLevel, uint toLevel)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (sgUser is null)
                return;

            var reward = new ItemReward();

            for (uint level = fromLevel + 1; level <= toLevel; level++)
            {
                if (level == 100u || level == 50u)
                    reward = new ItemReward().AddCapsules(1u);
                else if (level % 25u == 0u)
                    reward = new ItemReward().AddSpheres(1u);
                else if (level % 10u == 0u)
                    reward = new ItemReward().AddTokens(2u);
                else if (level % 5u == 0u)
                    reward = new ItemReward().AddCoins(2_000u).AddTickets(1u);
                else
                    reward = new ItemReward().AddCoins(2_000u).AddChests(1u);
            }

            await reward.DeliverToAsync(userId);
            await RiftBot.SendChatMessageAsync("levelup", new FormatData(userId)
            {
                Reward = reward
            });
        }

        public async Task<IonicMessage> GetUserCooldownsAsync(ulong userId)
        {
            return await RiftBot.GetMessageAsync("user-cooldowns", new FormatData(userId));
        }
        
        public async Task<IonicMessage> GetUserProfileAsync(ulong userId)
        {
            return await RiftBot.GetMessageAsync("user-profile", new FormatData(userId));
        }

        public async Task<IonicMessage> GetUserGameStatAsync(ulong userId)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (sgUser is null)
                return MessageService.Error;

            var dbSummoner = await DB.LolData.GetAsync(userId);

            if (string.IsNullOrWhiteSpace(dbSummoner.PlayerUUID))
                return await RiftBot.GetMessageAsync("loldata-nodata", new FormatData(userId));

            (var summonerResult, var summoner) = await RiftBot.GetService<RiotService>()
                .GetSummonerByEncryptedSummonerIdAsync(dbSummoner.SummonerRegion, dbSummoner.SummonerId);

            if (summonerResult != RequestResult.Success)
                return MessageService.Error;

            (var requestResult, var leaguePositions) = await RiftBot.GetService<RiotService>()
                .GetLeaguePositionsByEncryptedSummonerIdAsync(dbSummoner.SummonerRegion, dbSummoner.SummonerId);

            if (requestResult != RequestResult.Success)
                return MessageService.Error;

            return await RiftBot.GetMessageAsync("loldata-stat-success", new FormatData(userId)
            {
                LolStat = new LolStatData
                {
                    Summoner = summoner,
                    SoloQueue = leaguePositions.FirstOrDefault(x => x.QueueType == "RANKED_SOLO_5x5"),
                    Flex5v5 = leaguePositions.FirstOrDefault(x => x.QueueType == "RANKED_FLEX_5x5"),
                }
            });
        }

        public async Task<IonicMessage> GetUserStatAsync(ulong userId)
        {
            var statistics = await DB.Statistics.GetAsync(userId);

            return await RiftBot.GetMessageAsync("user-stat", new FormatData(userId)
            {
                Statistics = statistics
            });
        }

        static async Task UpdateRatingAsync()
        {
            using (var context = new RiftContext())
            {
                var sortedIds = await context.Users
                    .OrderByDescending(x => x.Level)
                    .ThenByDescending(x => x.Experience)
                    .Select(x => x.UserId)
                    .ToListAsync();

                SortedRating = sortedIds;
            }
        }

        public async Task<IonicMessage> OpenChestAsync(ulong userId, uint amount)
        {
            await chestMutex.WaitAsync().ConfigureAwait(false);

            IonicMessage result;

            RiftBot.Log.Info($"Opening chest for {userId.ToString()}.");

            try
            {
                result = await OpenChestInternalAsync(userId, amount).ConfigureAwait(false);
            }
            finally
            {
                chestMutex.Release();
            }

            return result;
        }

        public async Task<IonicMessage> OpenChestAllAsync(ulong userId)
        {
            await chestMutex.WaitAsync().ConfigureAwait(false);

            IonicMessage result;

            RiftBot.Log.Info($"Opening all chests for {userId.ToString()}");

            try
            {
                var dbInventory = await DB.Inventory.GetAsync(userId);
                result = await OpenChestInternalAsync(userId, dbInventory.Chests).ConfigureAwait(false);
            }
            finally
            {
                chestMutex.Release();
            }

            return result;
        }

        static async Task<IonicMessage> OpenChestInternalAsync(ulong userId, uint amount)
        {
            var dbInventory = await DB.Inventory.GetAsync(userId);

            if (dbInventory.Chests < amount || amount == 0)
                return await RiftBot.GetMessageAsync("chests-nochests", new FormatData(userId));

            await DB.Inventory.RemoveAsync(userId, new InventoryData { Chests = amount });
            ChestsOpened?.Invoke(null, new ChestsOpenedEventArgs(userId, amount));

            var chest = new ChestReward();
            await chest.DeliverToAsync(userId);
            await DB.Statistics.AddAsync(userId, new StatisticData { ChestsOpened = amount });

            return await RiftBot.GetMessageAsync("chests-open-success", new FormatData(userId));
        }

        public async Task<IonicMessage> OpenCapsuleAsync(ulong userId)
        {
            await capsuleMutex.WaitAsync().ConfigureAwait(false);

            IonicMessage result;

            RiftBot.Log.Info($"Opening capsule for {userId.ToString()}.");

            try
            {
                result = await OpenCapsuleInternalAsync(userId).ConfigureAwait(false);
            }
            finally
            {
                capsuleMutex.Release();
            }

            return result;
        }

        static async Task<IonicMessage> OpenCapsuleInternalAsync(ulong userId)
        {
            var dbUserInventory = await DB.Inventory.GetAsync(userId);

            if (dbUserInventory.Capsules == 0u)
                await RiftBot.GetMessageAsync("capsules-nocapsules", new FormatData(userId));

            await DB.Inventory.RemoveAsync(userId, new InventoryData { Capsules = 1u });

            var capsule = new CapsuleReward();
            await capsule.DeliverToAsync(userId);
            await DB.Statistics.AddAsync(userId, new StatisticData { CapsulesOpened = 1u });

            return await RiftBot.GetMessageAsync("capsules-open-success", new FormatData(userId));
        }

        public async Task<IonicMessage> OpenSphereAsync(ulong userId)
        {
            await sphereMutex.WaitAsync().ConfigureAwait(false);

            IonicMessage result;

            RiftBot.Log.Info($"Opening sphere for {userId.ToString()}.");

            try
            {
                result = await OpenSphereInternalAsync(userId).ConfigureAwait(false);
            }
            finally
            {
                sphereMutex.Release();
            }

            return result;
        }

        static async Task<IonicMessage> OpenSphereInternalAsync(ulong userId)
        {
            var dbInventory = await DB.Inventory.GetAsync(userId);

            if (dbInventory.Spheres == 0u)
                return await RiftBot.GetMessageAsync("spheres-nospheres", new FormatData(userId));

            await DB.Inventory.RemoveAsync(userId, new InventoryData { Spheres = 1u });
            OpenedSphere?.Invoke(null, new OpenedSphereEventArgs(userId));

            var sphere = new SphereReward();
            await sphere.DeliverToAsync(userId);
            await DB.Statistics.AddAsync(userId, new StatisticData { SpheresOpened = 1u });

            return await RiftBot.GetMessageAsync("spheres-open-success", new FormatData(userId));
        }

        public async Task ActivateDoubleExp(ulong userId)
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Comms, out var channel))
                return;

            var dbInventory = await DB.Inventory.GetAsync(userId);

            if (dbInventory.BonusDoubleExp == 0)
            {
                var msg = await RiftBot.GetMessageAsync("activate-nopowerup", new FormatData(userId));
                await channel.SendIonicMessageAsync(msg);
                return;
            }

            var dbDoubleExp = await DB.Cooldowns.GetAsync(userId);
            if (dbDoubleExp.DoubleExpTime > DateTime.UtcNow)
            {
                var msg = await RiftBot.GetMessageAsync("activate-active", new FormatData(userId));
                await channel.SendIonicMessageAsync(msg);
                return;
            }

            await DB.Inventory.RemoveAsync(userId, new InventoryData { DoubleExps = 1 });

            var dateTime = DateTime.UtcNow.AddHours(12.0);
            await DB.Cooldowns.SetDoubleExpTimeAsync(userId, dateTime);

            var msgSuccess = await RiftBot.GetMessageAsync("activate-success-doubleexp", new FormatData(userId));
            await channel.SendIonicMessageAsync(msgSuccess);
        }

        public async Task ActivateBotRespect(ulong userId)
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Comms, out var channel))
                return;

            var dbInventory = await DB.Inventory.GetAsync(userId);

            if (dbInventory.BonusBotRespect == 0)
            {
                var msgNoPowerUp = await RiftBot.GetMessageAsync("activate-nopowerup", new FormatData(userId));
                await channel.SendIonicMessageAsync(msgNoPowerUp);
                return;
            }

            var dbCooldowns = await DB.Cooldowns.GetAsync(userId);
            if (dbCooldowns.BotRespectTime > DateTime.UtcNow)
            {
                var msgActive = await RiftBot.GetMessageAsync("activate-active", new FormatData(userId));
                await channel.SendIonicMessageAsync(msgActive);
                return;
            }

            await DB.Inventory.RemoveAsync(userId, new InventoryData { BotRespects = 1 });
            ActivatedBotRespects?.Invoke(null, new ActivatedBotRespectsEventArgs(userId));

            var dateTime = DateTime.UtcNow.AddHours(12.0);
            await DB.Cooldowns.SetBotRespeсtTimeAsync(userId, dateTime);

            var msgSuccess = await RiftBot.GetMessageAsync("activate-success-botrespect", new FormatData(userId));
            await channel.SendIonicMessageAsync(msgSuccess);
        }

        public static uint GetExpForLevel(uint level)
        {
            return (uint) (Math.Pow(level, 1.5) * 40 - 40);
        }
    }

    public enum Currency
    {
        Coins,
        Tokens,
    }
}
