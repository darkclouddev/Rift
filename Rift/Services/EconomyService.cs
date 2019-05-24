using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data;
using Rift.Services.Economy;
using Rift.Services.Message;
using Rift.Services.Reward;
using Rift.Util;

using Discord;
using Discord.WebSocket;
using Humanizer;
using IonicLib;
using IonicLib.Extensions;
using IonicLib.Util;
using Microsoft.EntityFrameworkCore;

namespace Rift.Services
{
    public class EconomyService
    {
        public static readonly Dictionary<ulong, string> TempRoles = new Dictionary<ulong, string>
        {
            {
                Settings.RoleId.Arcade, "Аркадные"
            },
            {
                Settings.RoleId.Arclight, "Светоносные"
            },
            {
                Settings.RoleId.BloodMoon, "Кровавая луна"
            },
            {
                Settings.RoleId.BravePoro, "Храбрые поро"
            },
            {
                Settings.RoleId.Chosen, "Избранные"
            },
            {
                Settings.RoleId.DarkStar, "Темная звезда"
            },
            {
                Settings.RoleId.Debonairs, "Галантные"
            },
            {
                Settings.RoleId.Epic, "Эпические"
            },
            {
                Settings.RoleId.HappyPoro, "Довольные поро"
            },
            {
                Settings.RoleId.Hextech, "Хекстековые"
            },
            {
                Settings.RoleId.Justicars, "Юстициары"
            },
            {
                Settings.RoleId.Mythic, "Мифические"
            },
            {
                Settings.RoleId.Party, "Тусовые"
            },
            {
                Settings.RoleId.Pentakill, "Pentakill"
            },
            {
                Settings.RoleId.StarGuardians, "Звездные защитники"
            },
            {
                Settings.RoleId.ThunderLords, "Повелители грома"
            },
            {
                Settings.RoleId.Vandals, "Вандалы"
            },
            {
                Settings.RoleId.Victorious, "Победоносные"
            },
            {
                Settings.RoleId.Ward, "Вардилочка"
            },
            {
                Settings.RoleId.Reworked, "Реворкнутый"
            },
            {
                Settings.RoleId.Meta, "Метовый"
            },
            {
                Settings.RoleId.Hasagi, "Хасаги"
            },
            {
                Settings.RoleId.YasuoPlayer, "Ясуоплееры"
            },
        };

        static Timer ratingUpdateTimer;
        static Timer ActiveUsersTimer;
        static Timer RichUsersTimer;
        static readonly TimeSpan ratingTimerCooldown = TimeSpan.FromHours(1);
        static List<ulong> ratingSorted = null;

        static SemaphoreSlim chestMutex = new SemaphoreSlim(1);
        static SemaphoreSlim capsuleMutex = new SemaphoreSlim(1);
        static SemaphoreSlim sphereMutex = new SemaphoreSlim(1);
        static SemaphoreSlim storeMutex = new SemaphoreSlim(1);
        static SemaphoreSlim giftMutex = new SemaphoreSlim(1);
        static SemaphoreSlim attackMutex = new SemaphoreSlim(1);
        static SemaphoreSlim dailyChestMutex = new SemaphoreSlim(1);
        static SemaphoreSlim bragMutex = new SemaphoreSlim(1);

        public void Init()
        {
            ratingUpdateTimer = new Timer(async delegate { await UpdateRatingAsync(); }, null, TimeSpan.FromMinutes(5), ratingTimerCooldown);
            InitActiveUsersTimer();
            InitRichUsersTimer();
        }

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

            var topTen = await Database.GetTopTenByExpAsync(x => 
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

            var topTen = await Database.GetTopTenByCoinsAsync(x => 
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
            await AddExpAsync(message.Author.Id, 1u);
        }

        static async Task AddExpAsync(ulong userId, uint exp)
        {
            await Database.AddExperienceAsync(userId, exp).ContinueWith(async _ =>
            {
                await CheckLevelUpAsync(userId);
            });
        }

        static async Task CheckLevelUpAsync(ulong userId)
        {
            var dbUserLevel = await Database.GetUserLevelAsync(userId);

            if (dbUserLevel.Experience != uint.MaxValue)
            {
                var newLevel = dbUserLevel.Level + 1u;

                while (dbUserLevel.Experience >= GetExpForLevel(newLevel))
                {
                    newLevel++;
                }

                newLevel--;

                if (newLevel > dbUserLevel.Level)
                {
                    await Database.SetLevelAsync(userId, newLevel);

                    RiftBot.Log.Info($"{userId.ToString()} just leveled up to {newLevel.ToString()}");

                    if (newLevel == 1u)
                        return; //no rewards on level 1

                    if (IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Comms, out var commsChannel))
                    {
                        var msg = await RiftBot.GetMessageAsync("levelup", new FormatData(userId));

                        await commsChannel.SendIonicMessageAsync(msg);
                    }
                    
                    await GiveRewardsForLevelAsync(userId, dbUserLevel.Level, newLevel);
                }
            }

            await CheckDailyMessageAsync(userId);
        }

        static async Task CheckDailyMessageAsync(ulong userId)
        {
            return;

            try
            {
                var dbDaily = await Database.GetUserLastDailyChestTimeAsync(userId);
    
                var diff = DateTime.UtcNow - dbDaily.LastDailyChestTime;
    
                if (diff.Days == 0)
                    return;
            
                await dailyChestMutex.WaitAsync().ConfigureAwait(false);

                var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

                if (sgUser is null)
                    return;

                var coins = Helper.NextUInt(1000, 2001);
                var tokens = 0u;

                if (IonicClient.HasRolesAny(sgUser, Settings.RoleId.Legendary))
                    coins += 1000u;
                if (IonicClient.HasRolesAny(sgUser, Settings.RoleId.Absolute))
                    coins += 2000u;
                if (IonicClient.HasRolesAny(sgUser, Settings.RoleId.Keepers))
                    tokens += 1u;
                if (IonicClient.HasRolesAny(sgUser, Settings.RoleId.Active))
                    coins += 500u;

                var reward = new ItemReward().AddCoins(coins).AddTokens(tokens);

                await reward.DeliverToAsync(userId);
                await Database.SetLastDailyChestTimeAsync(userId, DateTime.UtcNow);

                var msg = await RiftBot.GetMessageAsync("daily-reward", new FormatData(userId));

                RiftBot.GetService<MessageService>().TryAddSend(
                    new MixedMessage(DestinationType.GuildChannel, Settings.ChannelId.Comms, TimeSpan.Zero, msg));
            }
            finally
            {
                dailyChestMutex.Release();
            }
        }

        static async Task GiveRewardsForLevelAsync(ulong userId, uint fromLevel, uint toLevel)
        {
            for (uint level = fromLevel + 1; level <= toLevel; level++)
            {
                await GiveRewardsForLevelAsync(userId, level);
            }
        }

        static async Task GiveRewardsForLevelAsync(ulong userId, uint level)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (sgUser is null)
                return;

            ItemReward reward;

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

            await reward.DeliverToAsync(userId);
        }

        public async Task<IonicMessage> GetUserCooldownsAsync(ulong userId)
        {
            return await RiftBot.GetMessageAsync("user-cooldowns", new FormatData(userId));
        }
        
        public async Task<IonicMessage> GetUserProfileAsync(ulong userId)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (sgUser is null)
                return MessageService.Error;

            var profile = await Database.GetUserProfileAsync(userId);

            var position = ratingSorted is null
                ? "-"
                : $"{(ratingSorted.IndexOf(userId) + 1).ToString()} / {ratingSorted.Count.ToString()}";

            var tempRoles = await RiftBot.GetService<RoleService>().GetUserTempRolesAsync(userId);
            var tempRolesList = new List<string>();

            foreach (var role in tempRoles)
            {
                if (!TempRoles.ContainsKey(role.RoleId) && role.RoleId != Settings.RoleId.Keepers)
                    continue;

                var timeLeft = role.ExpirationTime - DateTime.UtcNow;
                
                tempRolesList.Add($"- {TempRoles[role.RoleId]} ({timeLeft.Humanize()})");
            }

            var tempRolesString = tempRolesList.Any()
                ? string.Join('\n', tempRolesList)
                : "У вас нет временных ролей.";

            var currentLevelExp = GetExpForLevel(profile.Level);
            var fullLevelExp = GetExpForLevel(profile.Level+1u) - currentLevelExp;
            var remainingExp = fullLevelExp - (profile.Experience - currentLevelExp);
            var levelPerc = 100 - (int)Math.Floor(((float)remainingExp / fullLevelExp * 100));

            var embed = new RiftEmbed()
                .WithTitle("Ваш профиль")
                .WithThumbnailUrl(sgUser.GetAvatarUrl())
                .WithDescription($"Статистика и информация о вашем аккаунте в системе:")
                .AddField("Уровень", $"{Settings.Emote.LevelUp} {profile.Level.ToString()}", true)
                .AddField("Место", $"{Settings.Emote.Rewards} {position}", true)
                .AddField("Статистика текущего уровня",
                    $"{Settings.Emote.Experience} Получено {levelPerc.ToString()}% опыта до {(profile.Level+1).ToString()} уровня.")
                .AddField("Временные роли", tempRolesString);
            
            return new IonicMessage(embed.ToEmbed());
        }

        public async Task<IonicMessage> GetUserInventoryAsync(ulong userId)
        {
            return await RiftBot.GetMessageAsync("user-inventory", new FormatData(userId));
        }

        public async Task<IonicMessage> GetUserGameStatAsync(ulong userId)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (sgUser is null)
                return MessageService.Error;

            var dbSummoner = await Database.GetUserLolDataAsync(userId);

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
            return await RiftBot.GetMessageAsync("user-stat", new FormatData(userId));
        }

        public async Task<IonicMessage> GetUserBragAsync(ulong userId)
        {
            await bragMutex.WaitAsync().ConfigureAwait(false);

            IonicMessage result;

            try
            {
                result = await GetUserBragInternalAsync(userId).ConfigureAwait(false);
            }
            finally
            {
                bragMutex.Release();
            }

            return result;
        }

        static async Task<IonicMessage> GetUserBragInternalAsync(ulong userId)
        {
            (var canBrag, var remaining) = await CanBrag(userId);

            if (!canBrag)
                return await RiftBot.GetMessageAsync("brag-cooldown", new FormatData(userId));

            var dbSummoner = await Database.GetUserLolDataAsync(userId);

            if (dbSummoner is null || string.IsNullOrWhiteSpace(dbSummoner.AccountId))
                return await RiftBot.GetMessageAsync("loldata-nodata", new FormatData(userId));

            (var matchlistResult, var matchlist) = await RiftBot.GetService<RiotService>()
                .GetLast20MatchesByAccountIdAsync(dbSummoner.SummonerRegion, dbSummoner.AccountId);

            if (matchlistResult != RequestResult.Success)
                return await RiftBot.GetMessageAsync("brag-nomatches", new FormatData(userId));

            (var matchDataResult, var matchData) = await RiftBot.GetService<RiotService>()
                .GetMatchById(dbSummoner.SummonerRegion, matchlist.Random().GameId);

            if (matchDataResult != RequestResult.Success)
            {
                RiftBot.Log.Error("Failed to get match data");
                return MessageService.Error;
            }

            long participantId = matchData.ParticipantIdentities
                .First(x => x.Player.CurrentAccountId == dbSummoner.AccountId || x.Player.AccountId == dbSummoner.AccountId)
                .ParticipantId;

            var player = matchData.Participants.First(x => x.ParticipantId == participantId);

            if (player is null)
            {
                RiftBot.Log.Error("Failed to get player object");
                return MessageService.Error;
            }

            var champData = RiftBot.GetService<RiotService>().GetChampionById(player.ChampionId.ToString());

            if (champData is null)
            {
                RiftBot.Log.Error("Failed to obtain champ data");
                return MessageService.Error;
            }

            var champThumb = RiotService.GetChampionSquareByName(champData.Image);

            await Database.SetLastBragTimeAsync(userId, DateTime.UtcNow);

            var brag = new Brag(player.Stats.Win);
            await Database.AddInventoryAsync(userId, new InventoryData { Coins = brag.Coins });

            var queue = RiftBot.GetService<RiotService>().GetQueueNameById(matchData.QueueId);
            
            await Database.AddStatisticsAsync(userId, bragTotal: 1u);

            return await RiftBot.GetMessageAsync("brag-success", new FormatData(userId)
            {
                Brag = new BragData
                {
                    ChampionName = champData.Name,
                    ChampionPortraitUrl = champThumb,
                    Stats = player.Stats,
                    QueueName = queue,
                    Reward = brag.Coins.ToString(),
                }
            });
        }

        static async Task<(bool, TimeSpan)> CanBrag(ulong userId)
        {
            var data = await Database.GetUserLastBragTimeAsync(userId);

            if (data.LastBragTime == DateTime.MinValue)
                return (true, TimeSpan.Zero);

            var diff = DateTime.UtcNow - data.LastBragTime;

            var result = diff > Settings.Economy.BragCooldown;

            return (result, result ? TimeSpan.Zero : Settings.Economy.BragCooldown - diff);
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

                ratingSorted = sortedIds;
            }
        }

        public async Task<IonicMessage> OpenChestAsync(ulong userId)
        {
            await chestMutex.WaitAsync().ConfigureAwait(false);

            IonicMessage result;

            RiftBot.Log.Info($"Opening chest for {userId.ToString()}.");

            try
            {
                result = await OpenChestInternalAsync(userId, 1).ConfigureAwait(false);
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
                var dbInventory = await Database.GetUserInventoryAsync(userId);
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
            var dbInventory = await Database.GetUserInventoryAsync(userId);
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (dbInventory.Chests < amount || amount == 0)
                return await RiftBot.GetMessageAsync("chests-nochests", new FormatData(userId));

            await Database.RemoveInventoryAsync(userId, new InventoryData { Chests = amount });

            var chest = new ChestReward();
            await chest.DeliverToAsync(userId);
            await Database.AddStatisticsAsync(userId, chestsOpenedTotal: amount);

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
            var dbUserInventory = await Database.GetUserInventoryAsync(userId);

            if (dbUserInventory.Capsules == 0u)
                await RiftBot.GetMessageAsync("capsules-nocapsules", new FormatData(userId));

            await Database.RemoveInventoryAsync(userId, new InventoryData { Capsules = 1u });

            var capsule = new CapsuleReward();
            await capsule.DeliverToAsync(userId);
            await Database.AddStatisticsAsync(userId, capsuleOpenedTotal: 1u);

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
            var dbInventory = await Database.GetUserInventoryAsync(userId);

            if (dbInventory.Spheres == 0u)
                return await RiftBot.GetMessageAsync("spheres-nospheres", new FormatData(userId));

            await Database.RemoveInventoryAsync(userId, new InventoryData { Spheres = 1u });

            var sphere = new SphereReward();
            await sphere.DeliverToAsync(userId);
            await Database.AddStatisticsAsync(userId, sphereOpenedTotal: 1u);

            return await RiftBot.GetMessageAsync("spheres-open-success", new FormatData(userId));
        }

        public async Task<IonicMessage> StorePurchaseAsync(ulong userId, StoreItem item)
        {
            await storeMutex.WaitAsync().ConfigureAwait(false);

            RiftBot.Log.Info($"Store purchase: #{item.Id.ToString()} by {userId.ToString()}.");

            IonicMessage result;
            
            try
            {
                result = await StorePurchaseInternalAsync(userId, item).ConfigureAwait(false);
            }
            finally
            {
                storeMutex.Release();
            }

            return result;
        }

        static async Task<IonicMessage> StorePurchaseInternalAsync(ulong userId, StoreItem item)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (sgUser is null)
                return MessageService.Error;

            (var canPurchase, var remaining) = await CanBuyStoreAsync(userId);

            if (!RiftBot.IsAdmin(sgUser) && !canPurchase)
                await RiftBot.GetMessageAsync("store-cooldown", new FormatData(userId));

            // if buying temp role over existing one
            if (item.Type == StoreItemType.TempRole)
            {
                var userTempRoles = await RiftBot.GetService<RoleService>().GetUserTempRolesAsync(userId);

                if (userTempRoles != null && userTempRoles.Count > 0)
                {
                    if (userTempRoles.Any(x => x.UserId == userId && x.RoleId == item.RoleId))
                    {
                        return await RiftBot.GetMessageAsync("store-hasrole", new FormatData(userId));
                    }
                }
            }

            (var result, var currencyType) = await WithdrawCurrencyAsync();

            if (!result)
            {
                switch (currencyType)
                {
                    case Currency.Coins: return await RiftBot.GetMessageAsync("store-nocoins", new FormatData(userId));
                    case Currency.Tokens: return await RiftBot.GetMessageAsync("store-notokens", new FormatData(userId));
                }
            }

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Comms, out var channel))
                return MessageService.Error;

            switch (item.Type)
            {
                case StoreItemType.DoubleExp:
                    await Database.AddInventoryAsync(userId, new InventoryData { DoubleExps = 1u });
                    break;

                case StoreItemType.Capsule:
                    await Database.AddInventoryAsync(userId, new InventoryData { Capsules = 1u });
                    break;

                case StoreItemType.Ticket:
                    await Database.AddInventoryAsync(userId, new InventoryData{  Tickets = 1u });
                    break;

                case StoreItemType.Chest:
                    await Database.AddInventoryAsync(userId, new InventoryData { Chests = 1u });
                    break;

                case StoreItemType.Token:
                    await Database.AddInventoryAsync(userId, new InventoryData { Tokens = 1u });
                    break;

                case StoreItemType.Sphere:
                    await Database.AddInventoryAsync(userId, new InventoryData { Spheres = 1u });
                    break;

                case StoreItemType.BotRespect:
                    await Database.AddInventoryAsync(userId, new InventoryData { BotRespects = 1u });
                    break;

                case StoreItemType.TempRole:

                    await RiftBot.GetService<RoleService>()
                        .AddTempRoleAsync(userId, item.RoleId, TimeSpan.FromDays(30), "Store Purchase");

                    var msgTempRole = await RiftBot.GetMessageAsync("store-purchased-temprole", new FormatData(userId));
                    await channel.SendIonicMessageAsync(msgTempRole);
                    break;

                case StoreItemType.PermanentRole:
                    var msgPermRole = await RiftBot.GetMessageAsync("store-purchased-permrole", new FormatData(userId));
                    await channel.SendIonicMessageAsync(msgPermRole);
                    await RiftBot.GetService<RoleService>().AddPermanentRoleAsync(userId, item.RoleId);
                    break;
            }

            var balance = await GetBalanceString();

            async Task<(bool, Currency)> WithdrawCurrencyAsync()
            {
                var dbInventory = await Database.GetUserInventoryAsync(userId);

                switch (item.Currency)
                {
                    case Currency.Coins:
                    {
                        if (dbInventory.Coins < item.Price)
                            return (false, item.Currency);

                        await Database.RemoveInventoryAsync(userId, new InventoryData { Coins = item.Price } );
                        break;
                    }

                    case Currency.Tokens:
                    {
                        if (dbInventory.Tokens < item.Price)
                            return (false, item.Currency);

                        await Database.RemoveInventoryAsync(userId, new InventoryData { Tokens = item.Price });
                        break;
                    }
                }

                return (true, item.Currency);
            }

            await Database.SetLastStoreTimeAsync(userId, DateTime.UtcNow);
            await Database.AddStatisticsAsync(userId, purchasedItemsTotal: 1u);

            async Task<string> GetBalanceString()
            {
                var dbInventory = await Database.GetUserInventoryAsync(userId);

                return $"{Settings.Emote.Coin} {dbInventory.Coins.ToString()} {Settings.Emote.Token} {dbInventory.Tokens.ToString()}";
            }

            return await RiftBot.GetMessageAsync("store-success", new FormatData(userId));
        }

        static async Task<(bool, TimeSpan)> CanBuyStoreAsync(ulong userId)
        {
            var dbStore = await Database.GetUserLastStoreTimeAsync(userId);

            if (dbStore.LastStoreTime == DateTime.MinValue)
                return (true, TimeSpan.Zero);

            var diff = DateTime.UtcNow - dbStore.LastStoreTime;
            var remaining = Settings.Economy.StoreCooldown - diff;

            return (diff > Settings.Economy.StoreCooldown, remaining);
        }

        public async Task<IonicMessage> GiftAsync(SocketGuildUser fromUser, SocketGuildUser toUser, uint type)
        {
            await giftMutex.WaitAsync().ConfigureAwait(false);

            IonicMessage result;

            RiftBot.Log.Info($"Gift from {fromUser.Id.ToString()} to {toUser.Id.ToString()}.");

            try
            {
                result = await GiftInternalAsync(fromUser, toUser, type).ConfigureAwait(false);
            }
            finally
            {
                giftMutex.Release();
            }

            return result;
        }

        static async Task<IonicMessage> GiftInternalAsync(SocketGuildUser fromUser, SocketGuildUser toUser, uint type)
        {
            if (toUser.IsBot)
            {
                RiftBot.Log.Debug("[Gift] Target is bot.");
                return await RiftBot.GetMessageAsync("gift-target-bot", new FormatData(fromUser.Id));
            }
            
            if (fromUser.Id == toUser.Id)
            {
                RiftBot.Log.Debug("[Gift] Ouch, self-gift.");
                return await RiftBot.GetMessageAsync("gift-target-self", new FormatData(fromUser.Id));
            }

            (var canGift, var remainingTime) = await CanGift(fromUser.Id);

            if (!RiftBot.IsAdmin(fromUser) && !canGift)
                return await RiftBot.GetMessageAsync("gift-cooldown", new FormatData(fromUser.Id));

            var giftItem = new GiftReward();

            if (!await WithdrawCurrencyAsync())
            {
                switch (currencyType)
                {
                    case Currency.Coins: return await RiftBot.GetMessageAsync("gift-nocoins", new FormatData(fromUser.Id));
                    case Currency.Tokens: return await RiftBot.GetMessageAsync("gift-notokens", new FormatData(fromUser.Id));
                }
            }

            await Database.SetLastGiftTimeAsync(fromUser.Id, DateTime.UtcNow);
            await Database.AddStatisticsAsync(fromUser.Id, giftsSent: 1u);
            await Database.AddStatisticsAsync(toUser.Id, giftsReceived: 1u);

            var dbInventory = await Database.GetUserInventoryAsync(fromUser.Id);

            RiftBot.Log.Debug("[Gift] Success.");

            return await RiftBot.GetMessageAsync("gift-success", new FormatData(fromUser.Id)
            {

            });

            async Task<bool> WithdrawCurrencyAsync()
            {
                var dbInventory2 = await Database.GetUserInventoryAsync(fromUser.Id);

                switch (giftItem.Currency)
                {
                    case Currency.Coins:
                        {
                            if (dbInventory2.Coins < giftItem.Price)
                                return (false, giftItem.Currency);

                            await Database.RemoveInventoryAsync(fromUser.Id, new InventoryData { Coins = giftItem.Price });
                            break;
                        }

                    case Currency.Tokens:
                        {
                            if (dbInventory2.Tokens < giftItem.Price)
                                return (false, giftItem.Currency);

                            await Database.RemoveInventoryAsync(fromUser.Id, new InventoryData { Tokens = giftItem.Price });
                            break;
                        }
                }

                return true;
            }
        }

        static async Task<(bool, TimeSpan)> CanGift(ulong userId)
        {
            var data = await Database.GetUserLastGiftTimeAsync(userId);

            if (data.LastGiftTime == DateTime.MinValue)
                return (true, TimeSpan.Zero);

            var diff = DateTime.UtcNow - data.LastGiftTime;

            var result = diff > Settings.Economy.GiftCooldown;

            return (result, result ? TimeSpan.Zero : Settings.Economy.GiftCooldown - diff);
        }

        public async Task GiftSpecialAsync(ulong fromId, ulong toId, GiftSource giftSource)
        {
            var user = IonicClient.GetGuildUserById(Settings.App.MainGuildId, toId);
            var chatEmbed = new EmbedBuilder();
            var dmEmbed = new EmbedBuilder();

            switch (giftSource)
            {
                case GiftSource.Streamer:

                    await Database.AddInventoryAsync(toId, new InventoryData { Coins = 300u, Chests = 2u});

                    chatEmbed.WithDescription($"Стример <@{fromId.ToString()}> подарил {Settings.Emote.Coin} 300 {Settings.Emote.Chest} 2 призывателю <@{toId.ToString()}>");
                    break;

                case GiftSource.Moderator:

                    await Database.AddInventoryAsync(toId, new InventoryData { Coins = 100u, Chests = 1u });

                    chatEmbed.WithDescription($"Призыватель <@{toId.ToString()}> получил {Settings.Emote.Coin} 100 {Settings.Emote.Chest} 1");
                    break;

                case GiftSource.Voice:

                    var coins = Helper.NextUInt(50, 350);
                    await Database.AddInventoryAsync(toId, new InventoryData { Coins = coins });

                    chatEmbed.WithAuthor("Голосовые каналы", Settings.Emote.VoiceUrl)
                        .WithDescription($"Призыватель <@{toId.ToString()}> получил {Settings.Emote.Coin} {coins.ToString()} за активность.");

                    dmEmbed.WithAuthor("Голосовые каналы", Settings.Emote.VoiceUrl)
                        .WithDescription($"Вы получили {Settings.Emote.Coin} {coins.ToString()} за активность.");
                    break;
            }

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Comms, out var channel))
                return;

            await user.SendEmbedAsync(dmEmbed);
            await channel.SendEmbedAsync(chatEmbed.Build());
        }

        public async Task ActivateDoubleExp(ulong userId)
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Comms, out var channel))
                return;

            var dbInventory = await Database.GetUserInventoryAsync(userId);

            if (dbInventory.BonusDoubleExp == 0)
            {
                var msg = await RiftBot.GetMessageAsync("activate-nopowerup", new FormatData(userId));
                await channel.SendIonicMessageAsync(msg);
                return;
            }

            var dbDoubleExp = await Database.GetUserDoubleExpTimeAsync(userId);
            if (dbDoubleExp.DoubleExpTime > DateTime.UtcNow)
            {
                var msg = await RiftBot.GetMessageAsync("activate-active", new FormatData(userId));
                await channel.SendIonicMessageAsync(msg);
                return;
            }

            await Database.RemoveInventoryAsync(userId, new InventoryData { DoubleExps = 1 });

            var dateTime = DateTime.UtcNow.AddHours(12.0);
            await Database.SetDoubleExpTimeAsync(userId, dateTime);

            var msgSuccess = await RiftBot.GetMessageAsync("activate-success-doubleexp", new FormatData(userId));
            await channel.SendIonicMessageAsync(msgSuccess);
        }

        public async Task ActivateBotRespect(ulong userId)
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Comms, out var channel))
                return;

            var dbInventory = await Database.GetUserInventoryAsync(userId);

            if (dbInventory.BonusBotRespect == 0)
            {
                var msgNoPowerUp = await RiftBot.GetMessageAsync("activate-nopowerup", new FormatData(userId));
                await channel.SendIonicMessageAsync(msgNoPowerUp);
                return;
            }

            var dbUser = await Database.GetUserProfileAsync(userId);
            if (dbUser.BotRespectTime > DateTime.UtcNow)
            {
                var msgActive = await RiftBot.GetMessageAsync("activate-active", new FormatData(userId));
                await channel.SendIonicMessageAsync(msgActive);
                return;
            }

            await Database.RemoveInventoryAsync(userId, new InventoryData { BotRespects = 1 });

            var dateTime = DateTime.UtcNow.AddHours(12.0);
            await Database.SetBotRespeсtTimeAsync(userId, dateTime);

            var msgSuccess = await RiftBot.GetMessageAsync("activate-success-botrespect", new FormatData(userId));
            await channel.SendIonicMessageAsync(msgSuccess);
        }

        public static uint GetExpForLevel(uint level)
        {
            return (uint) (Math.Pow(level, 1.5) * 40 - 40);
        }

        public static string GetUserNameById(ulong userId)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (!(sgUser is null))
            {
                return string.IsNullOrWhiteSpace(sgUser.Nickname)
                    ? sgUser.Username
                    : sgUser.Nickname;
            }

            return "-";
        }
    }

    public enum GiftSource
    {
        Streamer,
        Voice,
        Moderator,
    }

    public enum Currency
    {
        Coins,
        Tokens,
    }
}
