using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Rift.Configuration;
using Rift.Data;
using Rift.Embeds;
using Rift.Events;
using Rift.Rewards;
using Rift.Services.Economy;
using Rift.Services.Message;

using IonicLib;
using IonicLib.Extensions;
using IonicLib.Util;

using Discord;
using Discord.WebSocket;

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

            ActiveUsersTimer = new Timer(async delegate { await ShowActiveUsersAsync(); }, null, today - DateTime.UtcNow,
                                         TimeSpan.FromDays(1));
        }

        void InitRichUsersTimer()
        {
            var today = DateTime.Today.AddHours(18);

            if (DateTime.UtcNow > today)
                today = today.AddDays(1);

            RichUsersTimer = new Timer(async delegate { await ShowRichUsersAsync(); }, null, today - DateTime.UtcNow,
                                       TimeSpan.FromDays(1));
        }

        public static async Task ShowActiveUsersAsync()
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
                return;

            var topTen = await Database.GetTopTenByExpAsync(x => 
                !(IonicClient.GetGuildUserById(Settings.App.MainGuildId, x.UserId) is null));

            if (topTen.Length == 0)
                return;

            await chatChannel.SendEmbedAsync(EconomyEmbeds.ActiveUsersEmbed(topTen));
        }

        public static async Task ShowRichUsersAsync()
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
                return;

            var topTen = await Database.GetTopTenByCoinsAsync(x => 
                !(IonicClient.GetGuildUserById(Settings.App.MainGuildId, x.UserId) is null));

            if (topTen.Length == 0)
                return;

            await chatChannel.SendEmbedAsync(EconomyEmbeds.RichUsersEmbed(topTen));
        }

        static void OnDonateAdded(DonateAddedEventArgs e)
        {
            Task.Run(async delegate
            {
                var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, e.UserId);

                if (sgUser is null)
                    return;

                ulong roleId = 0;
                var announceRole = false;
                var messageService = RiftBot.GetService<MessageService>();

                messageService.TryAddSend(new EmbedMessage(DestinationType.GuildChannel, Settings.ChannelId.Chat,
                    TimeSpan.Zero, DonateEmbeds.ChatDonateEmbed(e.UserId, e.DonateAmount)));

                if (e.DonateTotal >= 500M && e.DonateTotal < 2_000M)
                {
                    announceRole = !IonicClient.HasRolesAny(e.UserId, Settings.RoleId.Legendary);

                    if (announceRole)
                        roleId = Settings.RoleId.Legendary;
                }
                else if (e.DonateTotal >= 2_000M && e.DonateTotal < 10_000M)
                {
                    announceRole = !IonicClient.HasRolesAny(sgUser, Settings.RoleId.Absolute);

                    if (announceRole)
                        roleId = Settings.RoleId.Absolute;
                }
                else if (e.DonateTotal >= 10_000M)
                {
                    var embed = new EmbedBuilder()
                                .WithDescription($"Призыватель <@{e.UserId.ToString()}> получил личную роль\nза общую сумму пожертвований в размере **10000** рублей.")
                                .Build();

                    messageService.TryAddSend(new EmbedMessage(DestinationType.GuildChannel, Settings.ChannelId.Chat,
                                                               TimeSpan.Zero, embed));
                }

                if (roleId != 0)
                {
                    await RiftBot.GetService<RoleService>().AddPermanentRoleAsync(e.UserId, roleId);

                    if (announceRole)
                    {
                        messageService.TryAddSend(roleId == Settings.RoleId.Legendary
                            ? new EmbedMessage(DestinationType.GuildChannel,
                                Settings.ChannelId.Chat, TimeSpan.Zero,
                                DonateEmbeds.ChatDonateLegendaryRoleRewardEmbed(e.UserId))
                            : new EmbedMessage(DestinationType.GuildChannel,
                                Settings.ChannelId.Chat, TimeSpan.Zero,
                                DonateEmbeds.ChatDonateAbsoluteRoleRewardEmbed(e.UserId)));
                    }
                }
            });
        }

        public async Task ProcessMessageAsync(IUserMessage message)
        {
            await AddExpAsync(message.Author.Id, 1u);
            await Database.AddStatisticsAsync(message.Author.Id, messagesSentTotal: 1u);
        }

        static async Task AddExpAsync(ulong userId, uint exp)
        {
            await Database.AddExperienceAsync(userId, exp);

            var dbUserLevel = await Database.GetUserLevelAsync(userId);

            if (dbUserLevel.Experience != uint.MaxValue)
            {
                uint newLevel = dbUserLevel.Level + 1u;

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

                    if (IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat,
                                                   out var chatChannel))
                    {
                        await chatChannel.SendEmbedAsync(LevelUpEmbeds.Chat(userId, newLevel));
                    }

                    await CheckAchievementsAsync(userId, newLevel);

                    await GiveRewardsForLevelAsync(userId, dbUserLevel.Level, newLevel);
                }
            }

            await CheckDailyMessageAsync(userId);
        }

        static async Task CheckDailyMessageAsync(ulong userId)
        {
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

                uint coins = Helper.NextUInt(1000, 2001);
                uint chests = 0;
                uint tokens = 0;

                if (IonicClient.HasRolesAny(sgUser, Settings.RoleId.Legendary))
                    coins += 1000u;
                if (IonicClient.HasRolesAny(sgUser, Settings.RoleId.Absolute))
                    coins += 2000u;
                if (IonicClient.HasRolesAny(sgUser, Settings.RoleId.Keepers))
                    tokens += 1u;
                if (IonicClient.HasRolesAny(sgUser, Settings.RoleId.Active))
                    coins += 500u;

                var reward = new Reward(coins: coins, chests: chests, tokens: tokens);

                reward.CalculateReward();
                await reward.GiveRewardAsync(userId);
                await Database.SetLastDailyChestTimeAsync(userId, DateTime.UtcNow);
                reward.GenerateRewardString();

                var dailyChestEmbed = ProfileEmbeds.DailyMessageGiftEmbed(reward.RewardString);

                RiftBot.GetService<MessageService>()
                       .TryAddSend(new EmbedMessage(DestinationType.DM, userId, TimeSpan.Zero, dailyChestEmbed));
            }
            finally
            {
                dailyChestMutex.Release();
            }
        }

        static async Task CheckAchievementsAsync(ulong userId, uint level)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (sgUser is null)
                return;

            if (level == Settings.Economy.AttackMinimumLevel)
            {
                await sgUser.SendEmbedAsync(AttackEmbeds.AttacksUnlockedDM);

                if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
                    return;
                
                await chatChannel.SendEmbedAsync(AttackEmbeds.AttacksUnlockedChat(userId));
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

            uint chests = 1u;

            if (IonicClient.HasRolesAny(sgUser, Settings.RoleId.Legendary))
                chests += 1u;

            if (IonicClient.HasRolesAny(sgUser, Settings.RoleId.Absolute))
                chests += 2u;

            Reward reward;
            
            switch (level)
            {
                case 3:
                    reward = new Reward(coins: 1000, powerupsDoubleExp: 1, chests: chests);
                    break;
                case 5:
                    reward = new Reward(coins: 1000, powerupsBotRespect: 1, chests: chests);
                    break;
                case 7:
                    reward = new Reward(coins: 1000, customTickets: 2, chests: chests);
                    break;
                case 10:
                    reward = new Reward(coins: 1000, giveawayTickets: 1, chests: chests);
                    break;
                case 15:
                    reward = new Reward(coins: 2000, tokens: 4, chests: chests);
                    break;
                case 20:
                    reward = new Reward(experience: 100, customTickets: 4, chests: chests);
                    break;
                case 30:
                    reward = new Reward(coins: 2000, chests: 4 + chests);
                    break;
                case 40:
                    reward = new Reward(experience: 200, spheres: 1, chests: chests);
                    break;
                case 50:
                    reward = new Reward(coins: 2000, tokens: 10, chests: chests);
                    break;
                case 60:
                    reward = new Reward(experience: 300, giveawayTickets: 2, chests: chests);
                    break;
                case 70:
                    reward = new Reward(coins: 2000, chests: 10 + chests);
                    break;
                case 80:
                    reward = new Reward(experience: 100, chests: 15 + chests);
                    break;
                default:
                    reward = new Reward(chests: level % 10 == 0 ? chests + 5 : chests);
                    break;
            }

            reward.CalculateReward();
            await reward.GiveRewardAsync(userId);
            reward.GenerateRewardString();

            var statistics = await Database.GetUserStatisticsAsync(userId);
            var eb = LevelUpEmbeds.DM(level, statistics, reward.RewardString);

            await sgUser.SendEmbedAsync(eb);
        }

        public async Task<Embed> GetUserCooldownsAsync(ulong userId)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (sgUser is null)
                return GenericEmbeds.Error;

            var cooldowns = await Database.GetUserCooldownsAsync(userId);

            return CooldownEmbeds.DMEmbed(cooldowns);
        }
        
        public async Task<Embed> GetUserProfileAsync(ulong userId)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (sgUser is null)
                return GenericEmbeds.Error;

            var profile = await Database.GetUserProfileAsync(userId);

            string position = ratingSorted is null
                ? "-"
                : $"{(ratingSorted.IndexOf(userId) + 1).ToString()} / {ratingSorted.Count.ToString()}";

            var tempRoles = await RiftBot.GetService<RoleService>().GetUserTempRolesAsync(userId);
            var donateRoleString = $"{Settings.Emote.Donate} Узнайте больше о донате: <#{Settings.ChannelId.Donate.ToString()}>";
            var tempRolesList = new List<string>();

            foreach (var role in tempRoles)
            {
                if (!TempRoles.ContainsKey(role.RoleId) && role.RoleId != Settings.RoleId.Keepers)
                    continue;

                var timeLeft = role.ExpirationTime - DateTime.UtcNow;
                string time;

                if (timeLeft.Days > 0)
                    time = $"{timeLeft.Days.ToString()} дн.";
                else if (timeLeft.Hours > 0)
                    time = $"{timeLeft.Hours.ToString()} ч.";
                else if (timeLeft.Minutes > 0)
                    time = $"{timeLeft.Minutes.ToString()} мин.";
                else
                    time = $"{timeLeft.Seconds.ToString()} сек.";

                tempRolesList.Add($"- {TempRoles[role.RoleId]} ({time})");
            }

            var tempRolesString = tempRolesList.Any()
                ? String.Join('\n', tempRolesList)
                : "У вас нет временных ролей.";

            var currentLevelExp = GetExpForLevel(profile.Level);
            var fullLevelExp = GetExpForLevel(profile.Level+1u) - currentLevelExp;
            var remainingExp = fullLevelExp - (profile.Experience - currentLevelExp);
            var levelPerc = 100 - (Int32)Math.Floor(((Single)remainingExp / fullLevelExp * 100));

            var embed = new EmbedBuilder()
                .WithTitle($"Ваш профиль")
                .WithThumbnailUrl(sgUser.GetAvatarUrl())
                .WithDescription($"Статистика и информация о вашем аккаунте в системе:")
                .AddField("Уровень", $"{Settings.Emote.LevelUp} {profile.Level.ToString()}", true)
                .AddField("Место", $"{Settings.Emote.Rewards} {position}", true)
                .AddField("Статистика текущего уровня",
                    $"{Settings.Emote.Experience} Получено {levelPerc.ToString()}% опыта до {(profile.Level+1).ToString()} уровня.")

                //.AddField("Голосовой онлайн", $"{Settings.Emote.Voice} Данная функция отключена.")
                .AddField("Платные роли и пожертвования", $"Текущая: -\n"
                                                          + $"Необходимо задонатить: - рублей,\n"
                                                          + $"чтобы получить {Settings.Emote.Legendary} легендарные\n\n"
                                                          + $"{Settings.Emote.Donate} Общая сумма пожертвований: {profile.TotalDonate:0.00} рублей.")
                .AddField("Временные роли", tempRolesString)
                .Build();

            return embed;
        }

        public async Task<Embed> GetUserInventoryAsync(ulong userId)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (sgUser is null)
                return GenericEmbeds.Error;

            var inventory = await Database.GetUserInventoryAsync(userId);

            return ProfileEmbeds.InventoryEmbed(inventory);
        }

        public async Task<Embed> GetUserGameStatAsync(ulong userId)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (sgUser is null)
                return GenericEmbeds.Error;

            var dbSummoner = await Database.GetUserLolDataAsync(userId);

            if (String.IsNullOrWhiteSpace(dbSummoner.PlayerUUID))
                return StatEmbeds.NoRankEmbed;

            (var summonerResult, var summoner) =
                await RiftBot.GetService<RiotService>()
                             .GetSummonerByEncryptedSummonerIdAsync(dbSummoner.SummonerRegion, dbSummoner.SummonerId);

            if (summonerResult != RequestResult.Success)
                return GenericEmbeds.Error;

            var summonerLeagues = await RiftBot.GetService<RiotService>()
                                               .GetLeaguePositionsByEncryptedSummonerIdAsync(dbSummoner.SummonerRegion,
                                                                                             dbSummoner.SummonerId);

            if (summonerLeagues.Item1 != RequestResult.Success)
                return GenericEmbeds.Error;

            var league = summonerLeagues.Item2.FirstOrDefault(x => x.QueueType == "RANKED_SOLO_5x5");

            string soloqRanking;

            if (league is null)
                soloqRanking = "Недостаточно игр";
            else
            {
                int totalGames = league.Wins + league.Losses;
                int winratePerc = (int) Math.Round(((double) league.Wins / (double) totalGames) * 100);
                soloqRanking =
                    $"{RiotService.GetStatStringFromRank(RiotService.GetRankFromPosition(league))} {league.Rank} ({league.LeaguePoints.ToString()} LP) ({league.Wins.ToString()}W / {league.Losses.ToString()}L Winrate: {winratePerc.ToString()}%)";
            }

            var thumbnail = RiftBot.GetService<RiotService>().GetSummonerIconUrlById(summoner.ProfileIconId);

            return StatEmbeds.SuccessEmbed(thumbnail, summoner, soloqRanking);
        }

        public async Task<Embed> GetUserStatAsync(ulong userId)
        {
            var statistics = await Database.GetUserStatisticsAsync(userId);
            return StatEmbeds.UserStatisticsEmbed(statistics);
        }

        public async Task<(BragResult, Embed)> GetUserBragAsync(ulong userId)
        {
            await bragMutex.WaitAsync().ConfigureAwait(false);

            (BragResult, Embed) result;

            try
            {
                result = await GetUserBragInternalAsync(userId).ConfigureAwait(false);
                if (result.Item1 == BragResult.Success)
                    await Database.AddStatisticsAsync(userId, bragTotal: 1u);
            }
            finally
            {
                bragMutex.Release();
            }

            return result;
        }

        static async Task<(BragResult, Embed)> GetUserBragInternalAsync(ulong userId)
        {
            (bool canBrag, TimeSpan remaining) = await CanBrag(userId);

            if (!canBrag)
                return (BragResult.Error, BragEmbeds.Cooldown(remaining));

            var dbSummoner = await Database.GetUserLolDataAsync(userId);

            if (dbSummoner is null || String.IsNullOrWhiteSpace(dbSummoner.AccountId))
                return (BragResult.Error, BragEmbeds.NoData);

            (var matchlistResult, var matchlist) = await RiftBot
                                                         .GetService<RiotService>()
                                                         .GetLast20MatchesByAccountIdAsync(dbSummoner.SummonerRegion,
                                                                                           dbSummoner.AccountId);

            if (matchlistResult != RequestResult.Success)
                return (BragResult.Error, BragEmbeds.NoMatches);

            (var matchDataResult, var matchData) = await RiftBot
                                                         .GetService<RiotService>()
                                                         .GetMatchById(dbSummoner.SummonerRegion,
                                                                       matchlist.Random().GameId);

            if (matchDataResult != RequestResult.Success)
            {
                RiftBot.Log.Error($"Failed to get match data");
                return (BragResult.Error, GenericEmbeds.Error);
            }

            long participantId = matchData.ParticipantIdentities
                                          .First(x => x.Player.CurrentAccountId == dbSummoner.AccountId
                                                      || x.Player.AccountId == dbSummoner.AccountId)
                                          .ParticipantId;

            var player = matchData.Participants
                                  .First(x => x.ParticipantId == participantId);

            if (player is null)
            {
                RiftBot.Log.Error($"Failed to get player object");
                return (BragResult.Error, GenericEmbeds.Error);
            }

            var champData = RiftBot.GetService<RiotService>().GetChampionById(player.ChampionId.ToString());

            if (champData is null)
            {
                RiftBot.Log.Error($"Failed to obtain champ data");
                return (BragResult.Error, GenericEmbeds.Error);
            }

            string champThumb = RiotService.GetChampionSquareByName(champData.Image);

            await Database.SetLastBragTimeAsync(userId, DateTime.UtcNow);

            var brag = new Brag(player.Stats.Win);
            await Database.AddInventoryAsync(userId, coins: brag.Coins);

            var queue = RiftBot.GetService<RiotService>().GetQueueNameById(matchData.QueueId);
            return (BragResult.Success, BragEmbeds.Success(userId, champThumb, champData.Name, player, queue,
                                                           brag.Coins));
        }

        static async Task<(bool, TimeSpan)> CanBrag(ulong userId)
        {
            var data = await Database.GetUserLastBragTimeAsync(userId);

            if (data.LastBragTime == DateTime.MinValue)
                return (true, TimeSpan.Zero);

            var diff = DateTime.UtcNow - data.LastBragTime;

            Boolean result = diff > Settings.Economy.BragCooldown;

            return (result, result
                        ? TimeSpan.Zero
                        : Settings.Economy.BragCooldown - diff);
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

        public async Task<(OpenChestResult, Embed)> OpenChestAsync(ulong userId)
        {
            await chestMutex.WaitAsync().ConfigureAwait(false);

            (OpenChestResult, Embed) result;

            RiftBot.Log.Info($"Opening chest for {userId.ToString()}.");

            try
            {
                result = await OpenChestInternalAsync(userId, 1).ConfigureAwait(false);
                
                if (result.Item1 == OpenChestResult.Success)
                    await Database.AddStatisticsAsync(userId, chestsOpenedTotal: 1);
            }
            finally
            {
                chestMutex.Release();
            }

            return result;
        }

        public async Task<(OpenChestResult, Embed)> OpenChestAllAsync(UInt64 userId)
        {
            await chestMutex.WaitAsync().ConfigureAwait(false);

            (OpenChestResult, Embed) result;

            RiftBot.Log.Info($"Opening all chests for {userId.ToString()}");

            try
            {
                var dbInventory = await Database.GetUserInventoryAsync(userId);
                result = await OpenChestInternalAsync(userId, dbInventory.Chests).ConfigureAwait(false);
                if (result.Item1 == OpenChestResult.Success)
                    await Database
                                 .AddStatisticsAsync(userId, chestsOpenedTotal: dbInventory.Chests);
            }
            finally
            {
                chestMutex.Release();
            }

            return result;
        }

        static async Task<(OpenChestResult, Embed)> OpenChestInternalAsync(ulong userId, uint amount)
        {
            var dbInventory = await Database.GetUserInventoryAsync(userId);
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (dbInventory.Chests < amount || amount == 0)
                return (OpenChestResult.NoChests, ChestEmbeds.NoChestsEmbed);

            await Database.RemoveInventoryAsync(userId, chests: amount);

            var chest = new Chest(userId, amount);

            await chest.GiveRewardAsync();
            await sgUser.SendEmbedAsync(ChestEmbeds.DMEmbed(chest.reward, amount));

            return (OpenChestResult.Success, GenericEmbeds.Empty);
        }

        public async Task<(OpenCapsuleResult, Embed)> OpenCapsuleAsync(ulong userId)
        {
            await capsuleMutex.WaitAsync().ConfigureAwait(false);

            (OpenCapsuleResult, Embed) result;

            RiftBot.Log.Info($"Opening capsule for {userId.ToString()}.");

            try
            {
                result = await OpenCapsuleInternalAsync(userId).ConfigureAwait(false);
                if (result.Item1 == OpenCapsuleResult.Success)
                    await Database.AddStatisticsAsync(userId, capsuleOpenedTotal: 1u);
            }
            finally
            {
                capsuleMutex.Release();
            }

            return result;
        }

        static async Task<(OpenCapsuleResult, Embed)> OpenCapsuleInternalAsync(ulong userId)
        {
            var dbUserInventory = await Database.GetUserInventoryAsync(userId);

            if (dbUserInventory.Capsules == 0u)
                return (OpenCapsuleResult.NoCapsules, CapsuleEmbeds.NoCapsulesEmbed);

            await Database.RemoveInventoryAsync(userId, capsules: 1u);

            var capsule = new Capsule(userId);
            await capsule.GiveRewardsAsync(userId);

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
                return (OpenCapsuleResult.Error, GenericEmbeds.Error);

            await chatChannel.SendEmbedAsync(CapsuleEmbeds.ChatEmbed(userId, capsule));

            return (OpenCapsuleResult.Success, CapsuleEmbeds.DMEmbed(capsule));
        }

        public async Task<(OpenSphereResult, Embed)> OpenSphereAsync(ulong userId)
        {
            await sphereMutex.WaitAsync().ConfigureAwait(false);

            (OpenSphereResult, Embed) result;

            RiftBot.Log.Info($"Opening sphere for {userId.ToString()}.");

            try
            {
                result = await OpenSphereInternalAsync(userId).ConfigureAwait(false);
                if (result.Item1 == OpenSphereResult.Success)
                    await Database.AddStatisticsAsync(userId, sphereOpenedTotal: 1u);
            }
            finally
            {
                sphereMutex.Release();
            }

            return result;
        }

        static async Task<(OpenSphereResult, Embed)> OpenSphereInternalAsync(ulong userId)
        {
            var dbInventory = await Database.GetUserInventoryAsync(userId);

            if (dbInventory.Spheres == 0u)
                return (OpenSphereResult.NoSpheres, SphereEmbeds.NoSpheresEmbed);

            await Database.RemoveInventoryAsync(userId, spheres: 1u);

            var sphere = new Sphere(userId);
            await sphere.GiveRewardsAsync(userId);

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
                return (OpenSphereResult.Error, GenericEmbeds.Error);

            await chatChannel.SendEmbedAsync(SphereEmbeds.ChatEmbed(userId, sphere));

            return (OpenSphereResult.Success, SphereEmbeds.DMEmbed(sphere));
        }

        public async Task<(StorePurchaseResult, Embed)> StorePurchaseAsync(ulong userId, StoreItem item)
        {
            await storeMutex.WaitAsync().ConfigureAwait(false);

            (StorePurchaseResult, Embed) result;

            RiftBot.Log.Info($"Store purchase: #{item.Id.ToString()} by {userId.ToString()}.");

            try
            {
                result = await StorePurchaseInternalAsync(userId, item).ConfigureAwait(false);
                if (result.Item1 == StorePurchaseResult.Success)
                    await Database.AddStatisticsAsync(userId, purchasedItemsTotal: 1u);
            }
            finally
            {
                storeMutex.Release();
            }

            return result;
        }

        static async Task<(StorePurchaseResult, Embed)> StorePurchaseInternalAsync(ulong userId, StoreItem item)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (sgUser is null)
                return (StorePurchaseResult.Error, GenericEmbeds.Error);

            (bool canBuy, TimeSpan remaining) = await CanBuyStoreAsync(userId);

            if (!RiftBot.IsAdmin(sgUser) && !canBuy)
                return (StorePurchaseResult.Cooldown, StoreEmbeds.Cooldown(remaining));

            // if buying temp role over existing one
            if (item.Type == StoreItemType.TempRole)
            {
                var userTempRoles = await RiftBot.GetService<RoleService>().GetUserTempRolesAsync(userId);

                if (userTempRoles != null && userTempRoles.Count > 0)
                {
                    if (userTempRoles.Any(x => x.UserId == userId && x.RoleId == item.RoleId))
                    {
                        return (StorePurchaseResult.HasRole, StoreEmbeds.HasRole);
                    }
                }
            }

            (bool result, var currencyType) = await WithdrawCurrencyAsync();

            if (!result)
            {
                switch (currencyType)
                {
                    case Currency.Coins: return (StorePurchaseResult.NoCoins, StoreEmbeds.NoCoins);

                    case Currency.Tokens: return (StorePurchaseResult.NoTokens, StoreEmbeds.NoTokens);
                }
            }

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var channel))
                return (StorePurchaseResult.Error, GenericEmbeds.Error);

            switch (item.Type)
            {
                case StoreItemType.DoubleExp:
                    await Database.AddInventoryAsync(userId, doubleExps: 1u);
                    break;

                case StoreItemType.Capsule:
                    await Database.AddInventoryAsync(userId, capsules: 1u);
                    break;

                case StoreItemType.UsualTicket:
                    await Database.AddInventoryAsync(userId, usualTickets: 1u);
                    break;

                case StoreItemType.RareTicket:
                    await Database.AddInventoryAsync(userId, rareTickets: 1u);
                    break;

                case StoreItemType.Chest:
                    await Database.AddInventoryAsync(userId, chests: 1u);
                    break;

                case StoreItemType.Token:
                    await Database.AddInventoryAsync(userId, tokens: 1u);
                    break;

                case StoreItemType.Sphere:
                    await Database.AddInventoryAsync(userId, spheres: 1u);
                    break;

                case StoreItemType.BotRespect:
                    await Database.AddInventoryAsync(userId, respects: 1u);
                    break;

                case StoreItemType.TempRole:

                    await RiftBot.GetService<RoleService>()
                                 .AddTempRoleAsync(userId, item.RoleId, TimeSpan.FromDays(30), "Store Purchase");
                    await channel.SendEmbedAsync(StoreEmbeds.Chat(sgUser, item));
                    break;

                case StoreItemType.PermanentRole:
                    await channel.SendEmbedAsync(StoreEmbeds.Chat(sgUser, item));
                    await RiftBot.GetService<RoleService>().AddPermanentRoleAsync(userId, item.RoleId);
                    break;
            }

            string balance = await GetBalanceString();

            async Task<(bool, Currency)> WithdrawCurrencyAsync()
            {
                var dbInventory = await Database.GetUserInventoryAsync(userId);

                switch (item.Currency)
                {
                    case Currency.Coins:
                    {
                        if (dbInventory.Coins < item.Price)
                            return (false, item.Currency);

                        await Database.RemoveInventoryAsync(userId, coins: item.Price);
                        break;
                    }

                    case Currency.Tokens:
                    {
                        if (dbInventory.Tokens < item.Price)
                            return (false, item.Currency);

                        await Database.RemoveInventoryAsync(userId, tokens: item.Price);
                        break;
                    }
                }

                return (true, item.Currency);
            }

            await Database.SetLastStoreTimeAsync(userId, DateTime.UtcNow);

            async Task<string> GetBalanceString()
            {
                var dbInventory = await Database.GetUserInventoryAsync(userId);

                return $"{Settings.Emote.Coin} {dbInventory.Coins.ToString()} {Settings.Emote.Token} {dbInventory.Tokens.ToString()}";
            }

            return (StorePurchaseResult.Success, StoreEmbeds.DMSuccess(item, balance));
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

        public async Task<(GiftResult, Embed)> GiftAsync(SocketGuildUser fromUser, SocketGuildUser toUser, uint type)
        {
            await giftMutex.WaitAsync().ConfigureAwait(false);

            (GiftResult, Embed) result;

            RiftBot.Log.Info($"Gift from {fromUser.Id.ToString()} to {toUser.Id.ToString()}.");

            try
            {
                result = await GiftInternalAsync(fromUser, toUser, type).ConfigureAwait(false);

                if (result.Item1 == GiftResult.Success)
                {
                    await Database.AddStatisticsAsync(fromUser.Id, giftsSent: 1u);
                    await Database.AddStatisticsAsync(toUser.Id, giftsReceived: 1u);
                }
            }
            finally
            {
                giftMutex.Release();
            }

            return result;
        }

        static async Task<(GiftResult, Embed)> GiftInternalAsync(SocketGuildUser fromUser, SocketGuildUser toUser, UInt32 type)
        {
            if (toUser.IsBot)
            {
                RiftBot.Log.Debug("[Gift] Target is bot.");
                return (GiftResult.TargetBot, GiftEmbeds.BotGift);
            }
            
            if (fromUser.Id == toUser.Id)
            {
                RiftBot.Log.Debug("[Gift] Ouch, self-gift.");
                return (GiftResult.SelfGift, GiftEmbeds.SelfGift);
            }

            (bool canGift, TimeSpan remainingTime) = await CanGift(fromUser.Id);

            if (!RiftBot.IsAdmin(fromUser) && !canGift)
                return (GiftResult.Cooldown, GiftEmbeds.Cooldown(remainingTime));

            var giftItem = Gift.GetGiftItemById(type);

            if (giftItem is null)
                return (GiftResult.NoItem, GiftEmbeds.IncorrectNumber);

            (bool result, var currencyType) = await WithdrawCurrencyAsync();

            if (!result)
            {
                switch (currencyType)
                {
                    case Currency.Coins: return (GiftResult.NoCoins, GiftEmbeds.NoCoins);

                    case Currency.Tokens: return (GiftResult.NoTokens, GiftEmbeds.NoTokens);
                }
            }

            await Database.SetLastGiftTimeAsync(fromUser.Id, DateTime.UtcNow);

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
                return (GiftResult.Error, GenericEmbeds.Error);

            string giftString = "";
            switch (giftItem.Type)
            {
                case GiftItemType.CoinsRandom:
                    uint coins = Helper.NextUInt(1000, 3501);
                    await Database.AddInventoryAsync(toUser.Id, coins: coins);
                    giftString = $"{Settings.Emote.Coin} {coins.ToString()}";
                    break;

                case GiftItemType.ChestsRandom:
                    uint chests = Helper.NextUInt(4, 12);
                    await Database.AddInventoryAsync(toUser.Id, chests: chests);
                    giftString = $"{Settings.Emote.Chest} {chests.ToString()}";
                    break;

                case GiftItemType.Chest:
                    await Database.AddInventoryAsync(toUser.Id, chests: 1);
                    giftString = $"{Settings.Emote.Chest} 1";
                    break;

                case GiftItemType.Sphere:
                    await Database.AddInventoryAsync(toUser.Id, spheres: 1u);
                    giftString = $"{Settings.Emote.Sphere} сферу";
                    break;

                case GiftItemType.UsualTicket:
                    await Database.AddInventoryAsync(toUser.Id, usualTickets: 1u);
                    giftString = $"{Settings.Emote.UsualTickets} обычный билет";
                    break;

                case GiftItemType.BotRespect:
                    await Database.AddInventoryAsync(toUser.Id, respects: 1);
                    giftString = $"{Settings.Emote.BotRespect} уважение ботов";
                    break;

                case GiftItemType.RareTicket:
                    await Database.AddInventoryAsync(toUser.Id, rareTickets: 1u);
                    giftString = $"{Settings.Emote.RareTickets} редкий билет";
                    break;

                case GiftItemType.DoubleExp:
                    await Database.AddInventoryAsync(toUser.Id, doubleExps: 1u);
                    giftString = $"{Settings.Emote.PowerupDoubleExperience} двойной опыт";
                    break;
            }

            var dbInventory = await Database.GetUserInventoryAsync(fromUser.Id);

            await fromUser.SendEmbedAsync(GiftEmbeds.DMFrom(toUser, dbInventory));
            await toUser.SendEmbedAsync(GiftEmbeds.DMTo(fromUser, giftString));

            var msg = await chatChannel.SendEmbedAsync(GiftEmbeds.Chat(fromUser, toUser, giftString));
            RiftBot.GetService<MessageService>().TryAddDelete(new DeleteMessage(msg, TimeSpan.FromMinutes(3)));

            RiftBot.Log.Debug("[Gift] Success.");

            return (GiftResult.Success, GenericEmbeds.Empty);

            async Task<(bool, Currency)> WithdrawCurrencyAsync()
            {
                var dbInventory2 = await Database.GetUserInventoryAsync(fromUser.Id);

                switch (giftItem.Currency)
                {
                    case Currency.Coins:
                    {
                        if (dbInventory2.Coins < giftItem.Price)
                            return (false, giftItem.Currency);

                        await Database.RemoveInventoryAsync(fromUser.Id, coins: giftItem.Price);
                        break;
                    }

                    case Currency.Tokens:
                    {
                        if (dbInventory2.Tokens < giftItem.Price)
                            return (false, giftItem.Currency);

                        await Database.RemoveInventoryAsync(fromUser.Id, tokens: giftItem.Price);
                        break;
                    }
                }

                return (true, giftItem.Currency);
            }
        }

        static async Task<(bool, TimeSpan)> CanGift(ulong userId)
        {
            var data = await Database.GetUserLastGiftTimeAsync(userId);

            if (data.LastGiftTime == DateTime.MinValue)
                return (true, TimeSpan.Zero);

            var diff = DateTime.UtcNow - data.LastGiftTime;

            bool result = diff > Settings.Economy.GiftCooldown;

            return (result, result
                        ? TimeSpan.Zero
                        : Settings.Economy.GiftCooldown - diff);
        }

        public async Task<(AttackResult, Embed)> AttackAsync(SocketGuildUser sgAttacker, SocketGuildUser sgTarget)
        {
            await attackMutex.WaitAsync().ConfigureAwait(false);

            (AttackResult, Embed) result;

            RiftBot.Log.Info($"Attacktime! {sgAttacker.Id.ToString()} want to kick {sgTarget.Id.ToString()}'s ass.");

            try
            {
                result = await AttackInternalAsync(sgAttacker, sgTarget).ConfigureAwait(false);
                
                if (result.Item1 == AttackResult.Success)
                {
                    await Database.AddStatisticsAsync(sgAttacker.Id, attacksDone: 1u);
                    await Database.AddStatisticsAsync(sgTarget.Id, attacksReceived: 1u);
                }
            }
            finally
            {
                attackMutex.Release();
            }

            return result;
        }

        static async Task<(AttackResult, Embed)> AttackInternalAsync(SocketGuildUser sgAttacker,
                                                                     SocketGuildUser sgTarget)
        {
            if (sgAttacker.Id == sgTarget.Id)
                return (AttackResult.SelfAttack, AttackEmbeds.SelfAttack);

            (bool canBeAttacked, TimeSpan canBeAttackedRemainingTime) = await CanBeAttackedAgain(sgTarget.Id);

            if (!canBeAttacked)
                return (AttackResult.TooOftenSame, AttackEmbeds.TargetCooldown(canBeAttackedRemainingTime));

            var attackerDbProfile = await Database.GetUserProfileAsync(sgAttacker.Id);
            var attackerDbInventory = await Database.GetUserInventoryAsync(sgAttacker.Id);

            if (attackerDbProfile.Level < Settings.Economy.AttackMinimumLevel)
                return (AttackResult.LowUserLevel, AttackEmbeds.LowLevel);

            (bool canAttack, TimeSpan canAttackRemainingTime) = await CanAttackAsync(sgAttacker.Id);

            if (!canAttack)
                return (AttackResult.OnUserCooldown, AttackEmbeds.Cooldown(canAttackRemainingTime));

            var targetDbUserProfile = await Database.GetUserProfileAsync(sgTarget.Id);
            var targetDbUserInventory = await Database.GetUserInventoryAsync(sgTarget.Id);

            if (targetDbUserProfile.Level < Settings.Economy.AttackMinimumLevel)
                return (AttackResult.LowTargetLevel, AttackEmbeds.LowTargetLevel);

            if (attackerDbInventory.Coins < Settings.Economy.AttackPrice)
                return (AttackResult.NoCoins, AttackEmbeds.NoCoins);

            await Database.RemoveInventoryAsync(sgAttacker.Id, coins: Settings.Economy.AttackPrice);

            var attack = new Attack(sgAttacker.Id, sgTarget.Id, targetDbUserInventory.Coins,
                                    targetDbUserInventory.Chests);

            switch (attack.Loot)
            {
                case AttackLoot.Coins:
                {
                    if (targetDbUserInventory.Coins < attack.Count)
                        attack.Count = targetDbUserInventory.Coins; //stealing target's last monies :(

                    await Database.RemoveInventoryAsync(sgTarget.Id, coins: attack.Count);
                    await Database.AddInventoryAsync(sgAttacker.Id, coins: attack.Count);
                    break;
                }
                case AttackLoot.Chests:
                {
                    if (targetDbUserInventory.Chests < attack.Count)
                        attack.Count = targetDbUserInventory.Chests; //stealing target's last chests :(

                    await Database.RemoveInventoryAsync(sgTarget.Id, chests: attack.Count);
                    await Database.AddInventoryAsync(sgAttacker.Id, chests: attack.Count);
                    break;
                }
                case AttackLoot.Mute:
                {
                    await RiftBot.GetService<RoleService>()
                                 .AddTempRoleAsync(sgTarget.Id, Settings.RoleId.Attacked,
                                                   TimeSpan.FromMinutes(attack.Count), "Attack mute");
                    break;
                }
                case AttackLoot.ReversedMute:
                {
                    await RiftBot.GetService<RoleService>()
                                 .AddTempRoleAsync(sgAttacker.Id, Settings.RoleId.Attacked,
                                                   TimeSpan.FromMinutes(attack.Count), "Attack mute");
                    break;
                }
                case AttackLoot.MutualMute:
                {
                    await RiftBot.GetService<RoleService>()
                                 .AddTempRoleAsync(sgTarget.Id, Settings.RoleId.Attacked,
                                                   TimeSpan.FromMinutes(attack.Count), "Attack mute");
                    await RiftBot.GetService<RoleService>()
                                 .AddTempRoleAsync(sgAttacker.Id, Settings.RoleId.Attacked,
                                                   TimeSpan.FromMinutes(attack.Count), "Attack mute");
                    break;
                }
            }

            await UpdateLastBeingAttackedSameCooldown(sgTarget.Id);

            await sgAttacker.SendEmbedAsync(AttackEmbeds.Attacker(sgTarget, attack));
            await sgTarget.SendEmbedAsync(AttackEmbeds.Target(sgAttacker, attack));

            await Database.SetLastAttackTimeAsync(sgAttacker.Id, DateTime.UtcNow); //attacker cd

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
                return (AttackResult.Error, GenericEmbeds.Error);

            var msg = await chatChannel.SendEmbedAsync(AttackEmbeds.Chat(sgAttacker, sgTarget));
            RiftBot.GetService<MessageService>().TryAddDelete(new DeleteMessage(msg, TimeSpan.FromMinutes(3)));

            msg = await chatChannel.SendEmbedAsync(AttackEmbeds.AttackDesc(sgAttacker, sgTarget, attack));
            RiftBot.GetService<MessageService>().TryAddDelete(new DeleteMessage(msg, TimeSpan.FromMinutes(3)));

            return (AttackResult.Success, GenericEmbeds.Empty);
        }

        static async Task<(bool, TimeSpan)> CanAttackAsync(ulong userId)
        {
            var dbAttack = await Database.GetUserLastAttackTimeAsync(userId);

            if (dbAttack.LastAttackTime == DateTime.MinValue)
                return (true, TimeSpan.Zero);

            var diff = DateTime.UtcNow - dbAttack.LastAttackTime;

            return (diff > Settings.Economy.AttackPerUserCooldown,
                    Settings.Economy.AttackPerUserCooldown - diff);
        }

        static async Task<(bool, TimeSpan)> CanBeAttackedAgain(ulong userId)
        {
            var dbAttacked = await Database.GetUserLastBeingAttackedTimeAsync(userId);

            if (dbAttacked.LastBeingAttackedTime == DateTime.MinValue) // no cd
                return (true, TimeSpan.Zero);

            var diff = DateTime.UtcNow - dbAttacked.LastBeingAttackedTime;

            bool result = diff > Settings.Economy.AttackSameUserCooldown;

            return (result, Settings.Economy.AttackSameUserCooldown - diff);
        }

        static async Task UpdateLastBeingAttackedSameCooldown(ulong userId)
        {
            await Database.SetLastBeingAttackedTimeAsync(userId, DateTime.UtcNow);
        }

        public async Task<GiftResult> GiftSpecialAsync(ulong fromId, ulong toId, GiftSource giftSource)
        {
            var user = IonicClient.GetGuildUserById(Settings.App.MainGuildId, toId);
            var chatEmbed = new EmbedBuilder();
            var dmEmbed = new EmbedBuilder();

            switch (giftSource)
            {
                case GiftSource.Streamer:

                    await Database.AddInventoryAsync(toId, coins: 300u, chests: 2u);

                    chatEmbed
                        .WithDescription($"Стример <@{fromId.ToString()}> подарил {Settings.Emote.Coin} 300 {Settings.Emote.Chest} 2 призывателю <@{toId}>");
                    break;

                case GiftSource.Moderator:

                    await Database.AddInventoryAsync(toId, coins: 100u, chests: 1u);

                    chatEmbed
                        .WithDescription($"Призыватель <@{toId.ToString()}> получил {Settings.Emote.Coin} 100 {Settings.Emote.Chest} 1");
                    break;

                case GiftSource.Voice:

                    var coins = Helper.NextUInt(50, 350);
                    await Database.AddInventoryAsync(toId, coins: coins);

                    chatEmbed
                        .WithAuthor("Голосовые каналы", Settings.Emote.VoiceUrl)
                        .WithDescription($"Призыватель <@{toId.ToString()}> получил {Settings.Emote.Coin} {coins.ToString()} за активность.");

                    dmEmbed
                        .WithAuthor("Голосовые каналы", Settings.Emote.VoiceUrl)
                        .WithDescription($"Вы получили {Settings.Emote.Coin} {coins.ToString()} за активность.");
                    break;
            }

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var channel))
                return GiftResult.Error;

            await user.SendEmbedAsync(dmEmbed);
            await channel.SendEmbedAsync(chatEmbed.Build());

            return GiftResult.Success;
        }

        public async Task ActivateDoubleExp(ulong userId)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);
            var dbInventory = await Database.GetUserInventoryAsync(userId);

            if (dbInventory.PowerupsDoubleExperience == 0)
            {
                await sgUser.SendEmbedAsync(ActivateEmbeds.NoSuchPowerupEmbed);
                return;
            }

            var dbDoubleExp = await Database.GetUserDoubleExpTimeAsync(userId);
            if (dbDoubleExp.DoubleExpTime > DateTime.UtcNow)
            {
                await sgUser.SendEmbedAsync(ActivateEmbeds.DoubleExpNotExpiredEmbed);
                return;
            }

            await Database.RemoveInventoryAsync(userId, doubleExps: 1);

            var dateTime = DateTime.UtcNow.AddHours(12.0);
            await Database.SetDoubleExpTimeAsync(userId, dateTime);

            await sgUser.SendEmbedAsync(ActivateEmbeds.DoubleExpSuccessEmbed);

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
                return;

            await chatChannel.SendEmbedAsync(ActivateEmbeds.DoubleExpChatEmbed(sgUser));
        }

        public async Task ActivateBotRespect(ulong userId)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);
            var dbInventory = await Database.GetUserInventoryAsync(userId);

            if (dbInventory.PowerupsBotRespect == 0)
            {
                await sgUser.SendEmbedAsync(ActivateEmbeds.NoSuchPowerupEmbed);
                return;
            }

            var dbUser = await Database.GetUserProfileAsync(userId);
            if (dbUser.BotRespectTime > DateTime.UtcNow)
            {
                await sgUser.SendEmbedAsync(ActivateEmbeds.BotRespectNotExpiredEmbed);
                return;
            }

            await Database.RemoveInventoryAsync(userId, respects: 1);

            var dateTime = DateTime.UtcNow.AddHours(12.0);
            await Database.SetBotRespeсtTimeAsync(userId, dateTime);

            await sgUser.SendEmbedAsync(ActivateEmbeds.BotRespectSuccessEmbed);

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
                return;

            await chatChannel.SendEmbedAsync(ActivateEmbeds.BotRespectChatEmbed(sgUser));
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
                return String.IsNullOrWhiteSpace(sgUser.Nickname)
                    ? sgUser.Username
                    : sgUser.Nickname;
            }

            return "-";
        }
    }

    public enum OpenChestResult
    {
        Success,
        NoChests,
        Error,
    }

    public enum OpenCapsuleResult
    {
        Success,
        NoCapsules,
        Error,
    }

    public enum OpenSphereResult
    {
        Success,
        NoSpheres,
        Error,
    }

    public enum AttackResult
    {
        Error,
        TargetModerator,
        TargetWise,
        TargetIon,
        SelfAttack,
        TooOftenSame,
        LowUserLevel,
        LowTargetLevel,
        NoCoins,
        OnGlobalCooldown,
        OnUserCooldown,
        Success,
    }

    public enum StorePurchaseResult
    {
        Error,
        Cooldown,
        HasBotRespect,
        HasRole,
        NoCoins,
        NoTokens,
        Success,
    }

    public enum GiftResult
    {
        Error,
        TargetBot,
        Cooldown,
        SelfGift,
        NoCoins,
        NoTokens,
        NoItem,
        Success,
    }

    public enum GiftSource
    {
        Streamer,
        Voice,
        Moderator,
    }

    public enum BragResult
    {
        Error,
        Cooldown,
        Success,
    }

    public enum Currency
    {
        Coins,
        Tokens,
    }
}
