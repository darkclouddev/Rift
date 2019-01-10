using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Rift.Configuration;
using Rift.Data;
using Rift.Data.Models;
using Rift.Embeds;
using Rift.Events;
using Rift.Rewards;
using Rift.Services;
using Rift.Services.Economy;
using Rift.Services.Message;
using Rift.Services.Role;

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
                Settings.RoleId.Pentakill, "Pentakill"
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
            RiftBot.GetService<DatabaseService>().OnDonateAdded += OnDonateAdded;

            ratingUpdateTimer = new Timer(async delegate { await UpdateRatingAsync(); }, null, TimeSpan.FromMinutes(5),
                                          ratingTimerCooldown);
            InitActiveUsersTimer();
            InitRichUsersTimer();
            CalculateAchievementRewards();
        }

        static void InitActiveUsersTimer()
        {
            var today = DateTime.Today.AddHours(19);

            if (DateTime.Now > today)
                today = today.AddDays(1);

            ActiveUsersTimer = new Timer(async delegate { await ShowActiveUsersAsync(); }, null, today - DateTime.Now,
                                         TimeSpan.FromDays(1));
        }

        void InitRichUsersTimer()
        {
            var today = DateTime.Today.AddHours(21);

            if (DateTime.Now > today)
                today = today.AddDays(1);

            RichUsersTimer = new Timer(async delegate { await ShowRichUsersAsync(); }, null, today - DateTime.Now,
                                       TimeSpan.FromDays(1));
        }

        static void CalculateAchievementRewards()
        {
            Achievements.Write100Messages.CalculateReward();
            Achievements.Write100Messages.GenerateRewardString();

            Achievements.Write1000Messages.CalculateReward();
            Achievements.Write1000Messages.GenerateRewardString();

            Achievements.Reach10Level.CalculateReward();
            Achievements.Reach10Level.GenerateRewardString();

            Achievements.Reach30Level.CalculateReward();
            Achievements.Reach30Level.GenerateRewardString();

            Achievements.Brag100Times.CalculateReward();
            Achievements.Brag100Times.GenerateRewardString();

            Achievements.Attack200Times.CalculateReward();
            Achievements.Attack200Times.GenerateRewardString();

            Achievements.OpenSphere.CalculateReward();
            Achievements.OpenSphere.GenerateRewardString();

            Achievements.Purchase200Items.CalculateReward();
            Achievements.Purchase200Items.GenerateRewardString();

            Achievements.Open100Chests.CalculateReward();
            Achievements.Open100Chests.GenerateRewardString();

            Achievements.Send100Gifts.CalculateReward();
            Achievements.Send100Gifts.GenerateRewardString();

            Achievements.IsDonator.CalculateReward();
            Achievements.IsDonator.GenerateRewardString();

            Achievements.HasDonatedRole.CalculateReward();
            Achievements.HasDonatedRole.GenerateRewardString();

            Achievements.GiftToBotKeeper.CalculateReward();
            Achievements.GiftToBotKeeper.GenerateRewardString();

            Achievements.GiftToModerator.CalculateReward();
            Achievements.GiftToModerator.GenerateRewardString();

            Achievements.AttackWise.CalculateReward();
            Achievements.AttackWise.GenerateRewardString();
        }

        public static async Task ShowActiveUsersAsync()
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
                return;

            var topTen = await RiftBot.GetService<DatabaseService>()
                                      .GetTopTenByExpAsync(x => !(IonicClient.GetGuildUserById(Settings.App.MainGuildId,
                                                                                          x.UserId) is null));

            if (topTen.Length == 0)
                return;

            await chatChannel.SendEmbedAsync(EconomyEmbeds.ActiveUsersEmbed(topTen));
        }

        public static async Task ShowRichUsersAsync()
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
                return;

            var topTen = await RiftBot.GetService<DatabaseService>()
                                      .GetTopTenByCoinsAsync(x => !(IonicClient.GetGuildUserById(Settings.App.MainGuildId,
                                                                                            x.UserId) is null));

            if (topTen.Length == 0)
                return;

            await chatChannel.SendEmbedAsync(EconomyEmbeds.RichUsersEmbed(topTen));
        }

        public static async Task GetAchievements(ulong userId)
        {
            var achievements = await RiftBot.GetService<DatabaseService>().GetUserAchievementsAsync(userId);
            RiftBot.GetService<MessageService>()
                   .TryAddSend(new EmbedMessage(DestinationType.DM, userId, TimeSpan.Zero,
                                                AchievementEmbeds.AchievementsEmbed(achievements)));
        }

        public static async Task AddAchievement(ulong userId, string achievementName, Reward reward)
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
                return;

            await reward.GiveRewardAsync(userId);
            RiftBot.GetService<MessageService>()
                   .TryAddSend(new EmbedMessage(DestinationType.DM, userId, TimeSpan.Zero,
                                                AchievementEmbeds.DMEmbed(reward, achievementName)));
            await chatChannel.SendEmbedAsync(AchievementEmbeds.ChatEmbed(userId, achievementName));
        }

        public static async Task CheckAchievements(ulong userId)
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
                return;

            var statistic = await RiftBot.GetService<DatabaseService>().GetUserStatisticsAsync(userId);
            var achievements = await RiftBot.GetService<DatabaseService>().GetUserAchievementsAsync(userId);

            bool hasDonatedRole = false;

            var achievementsNew = new bool[11];
            var achievementsOld = new bool[]
            {
                achievements.Write100Messages,
                achievements.Write1000Messages,
                achievements.Reach10Level,
                achievements.Reach30Level,
                achievements.Brag100Times,
                achievements.Attack200Times,
                achievements.OpenSphere,
                achievements.Purchase200Items,
                achievements.Open100Chests,
                achievements.Send100Gifts,
                achievements.IsDonator
            };
            var statisticsList = new bool[]
            {
                statistic.MessagesSentTotal >= 100ul,
                statistic.MessagesSentTotal >= 1000ul,
                statistic.Level >= 10ul,
                statistic.Level >= 30ul,
                statistic.BragTotal >= 100ul,
                statistic.AttacksDone >= 200ul,
                statistic.SphereOpenedTotal >= 1ul,
                statistic.PurchasedItemsTotal >= 200ul,
                statistic.ChestsOpenedTotal >= 100ul,
                statistic.GiftsSent >= 100ul,
                statistic.Donate >= 1
            };
            var rewardList = new Reward[]
            {
                Achievements.Write100Messages,
                Achievements.Write1000Messages,
                Achievements.Reach10Level,
                Achievements.Reach30Level,
                Achievements.Brag100Times,
                Achievements.Attack200Times,
                Achievements.OpenSphere,
                Achievements.Purchase200Items,
                Achievements.Open100Chests,
                Achievements.Send100Gifts,
                Achievements.IsDonator
            };
            var achievementsNames = new string[]
            {
                "общительный",
                "без личной жизни",
                "активный",
                "постоянный",
                "хвастунишка",
                "линчеватель",
                "нечто классное",
                "транжира",
                "золотоискатель",
                "альтруист",
                "спонсор"
            };

            for (int i = 0; i < achievementsNew.Length; i++)
            {
                if (!achievementsOld[i] && statisticsList[i])
                {
                    achievementsNew[i] = true;
                    await AddAchievement(userId, achievementsNames[i], rewardList[i]);
                }
            }

            if (!achievements.HasDonatedRole)
            {
                var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

                if (!(sgUser is null)
                    && IonicClient.HasRolesAny(sgUser, Settings.RoleId.Legendary, Settings.RoleId.Absolute,
                                               Settings.RoleId.Keepers))
                {
                    hasDonatedRole = true;
                    await AddAchievement(userId, "несравненный", Achievements.HasDonatedRole);
                }
            }

            await RiftBot.GetService<DatabaseService>()
                         .UpdateAchievementAsync(userId,
                                                 write100Messages: achievementsNew[0],
                                                 write1000Messages: achievementsNew[1],
                                                 reach10Level: achievementsNew[2],
                                                 reach30Level: achievementsNew[3],
                                                 brag100Times: achievementsNew[4],
                                                 attack200Times: achievementsNew[5],
                                                 openSphere: achievementsNew[6],
                                                 purchase200Items: achievementsNew[7],
                                                 open100Chests: achievementsNew[8],
                                                 send100Gifts: achievementsNew[9],
                                                 isDonator: achievementsNew[10],
                                                 hasDonateRole: hasDonatedRole);
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
                                                           TimeSpan.Zero,
                                                           DonateEmbeds.ChatDonateEmbed(e.UserId, e.DonateAmount)));

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
                                .WithDescription($"Призыватель <@{e.UserId}> получил личную роль\nза общую сумму пожертвований в размере **10000** рублей.")
                                .Build();

                    messageService.TryAddSend(new EmbedMessage(DestinationType.GuildChannel, Settings.ChannelId.Chat,
                                                               TimeSpan.Zero, embed));
                }

                if (roleId != 0)
                {
                    await RiftBot.GetService<RoleService>().AddRoleAsync(e.UserId, roleId);

                    if (announceRole)
                    {
                        messageService.TryAddSend(roleId == Settings.RoleId.Legendary
                                                      ? new EmbedMessage(DestinationType.GuildChannel,
                                                                         Settings.ChannelId.Chat, TimeSpan.Zero,
                                                                         DonateEmbeds
                                                                             .ChatDonateLegendaryRoleRewardEmbed(e.UserId))
                                                      : new EmbedMessage(DestinationType.GuildChannel,
                                                                         Settings.ChannelId.Chat, TimeSpan.Zero,
                                                                         DonateEmbeds
                                                                             .ChatDonateAbsoluteRoleRewardEmbed(e.UserId)));
                    }
                }

                await CheckAchievements(e.UserId);
            });
        }

        public async Task ProcessMessageAsync(IUserMessage message)
        {
            await AddExpAsync(message.Author.Id, 1u);
            await RiftBot.GetService<DatabaseService>().AddStatisticsAsync(message.Author.Id, messagesSentTotal: 1ul);
            await CheckAchievements(message.Author.Id);
        }

        public static async Task AddExpAsync(ulong userId, uint exp)
        {
            await RiftBot.GetService<DatabaseService>().AddExperienceAsync(userId, exp);

            var dbUserLevel = await RiftBot.GetService<DatabaseService>().GetUserLevelAsync(userId);

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
                    await RiftBot.GetService<DatabaseService>().SetLevelAsync(userId, newLevel);

                    RiftBot.Log.Info($"{userId} just leveled up to {newLevel}");

                    if (newLevel == 1u)
                        return; //no rewards on level 1

                    if (IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat,
                                                   out var chatChannel))
                    {
                        await chatChannel.SendEmbedAsync(LevelUpEmbeds.ChatEmbed(userId, newLevel));
                    }

                    await CheckAchievementsAsync(userId, newLevel);

                    await GiveRewardsForLevelAsync(userId, dbUserLevel.Level, newLevel);
                }
            }

            await CheckDailyMessageAsync(userId);

            await CheckAchievements(userId);
        }

        const ulong secondsInDay = 60ul * 60ul * 24ul; //86400

        static async Task CheckDailyMessageAsync(ulong userId)
        {
            var dbDaily = await RiftBot.GetService<DatabaseService>().GetUserLastDailyChestTimestampAsync(userId);

            ulong diff = Helper.CurrentUnixTimestamp - dbDaily.LastDailyChestTimestamp;

            if (diff < secondsInDay)
                return;

            try
            {
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
                await RiftBot.GetService<DatabaseService>()
                             .SetLastDailyChestTimestampAsync(userId, Helper.CurrentUnixTimestamp);
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
                await sgUser.SendEmbedAsync(AttackEmbeds.AttacksUnlockedDMEmbed);

                if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat,
                                                out var chatChannel))
                    return;
                await chatChannel.SendEmbedAsync(AttackEmbeds.AttacksUnlockedChatEmbed(userId));
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

            Reward reward = null;
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

            var statistics = await RiftBot.GetService<DatabaseService>().GetUserStatisticsAsync(userId);
            var eb = LevelUpEmbeds.DMEmbed(level, statistics, reward.RewardString);

            await sgUser.SendEmbedAsync(eb);
        }

        public async Task<Embed> GetUserProfileAsync(ulong userId)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (sgUser is null)
                return GenericEmbeds.ErrorEmbed;

            var profile = await RiftBot.GetService<DatabaseService>().GetUserProfileAsync(userId);

            string position = ratingSorted is null
                ? "-"
                : $"{(ratingSorted.IndexOf(userId) + 1).ToString()} / {ratingSorted.Count}";

            var tempRoles = RiftBot.GetService<RoleService>().GetTempRoles(userId);
            var donateRoleString = $"{Settings.Emote.Donate} Узнайте больше о донате: <#{Settings.ChannelId.Donate}>";
            var tempRolesList = new List<string>();

            foreach (var role in tempRoles)
            {
                if (!TempRoles.ContainsKey(role.RoleId) && role.RoleId != Settings.RoleId.Keepers)
                    continue;

                ulong leftTime = role.UntilTimestamp - Helper.CurrentUnixTimestamp;
                string time;

                if (leftTime > 60 * 60 * 24)
                    time = $"{leftTime / (60 * 60 * 24)} дн.";
                else if (leftTime > 60 * 60)
                    time = $"{leftTime / (60 * 60)} ч.";
                else if (leftTime > 60)
                    time = $"{leftTime / 60} мин.";
                else
                    time = $"{leftTime} сек.";

                tempRolesList.Add($"- {TempRoles[role.RoleId]} ({time})");
            }

            var tempRolesString = tempRolesList.Any()
                ? String.Join('\n', tempRolesList)
                : "У вас нет временных ролей.";

            var embed = new EmbedBuilder()
                        .WithTitle($"Ваш профиль")
                        .WithThumbnailUrl(sgUser.GetAvatarUrl())
                        .WithDescription($"Статистика и информация о вашем аккаунте в системе:")
                        .AddField("Уровень", $"{Settings.Emote.LevelUp} {profile.Level}", true)
                        .AddField("Место", $"{Settings.Emote.Rewards} {position}", true)
                        .AddField("Статистика текущего уровня",
                                  $"{Settings.Emote.Experience} Получено "
                                  + $"{100 - (100 * (GetExpForLevel(profile.Level + 1u) - profile.Experience)) / (GetExpForLevel(profile.Level + 1u) - GetExpForLevel(profile.Level))}"
                                  + $"% опыта до {profile.Level + 1} уровня.")

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
                return GenericEmbeds.ErrorEmbed;

            var inventory = await RiftBot.GetService<DatabaseService>().GetUserInventoryAsync(userId);

            return ProfileEmbeds.InventoryEmbed(inventory);
        }

        public async Task<Embed> GetUserGameStatAsync(ulong userId)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (sgUser is null)
                return GenericEmbeds.ErrorEmbed;

            var dbSummoner = await RiftBot.GetService<DatabaseService>().GetUserLolDataAsync(userId);

            if (String.IsNullOrWhiteSpace(dbSummoner.PlayerUUID))
                return StatEmbeds.NoRankEmbed;

            (var summonerResult, var summoner) =
                await RiftBot.GetService<RiotService>()
                             .GetSummonerByEncryptedSummonerIdAsync(dbSummoner.SummonerRegion, dbSummoner.SummonerId);

            if (summonerResult != RequestResult.Success)
                return GenericEmbeds.ErrorEmbed;

            var summonerLeagues = await RiftBot.GetService<RiotService>()
                                               .GetLeaguePositionsByEncryptedSummonerIdAsync(dbSummoner.SummonerRegion,
                                                                                             dbSummoner.SummonerId);

            if (summonerLeagues.Item1 != RequestResult.Success)
                return GenericEmbeds.ErrorEmbed;

            var league = summonerLeagues.Item2.FirstOrDefault(x => x.QueueType == "RANKED_SOLO_5x5");

            string soloqRanking;

            if (league is null)
                soloqRanking = "Недостаточно игр";
            else
            {
                int totalGames = league.Wins + league.Losses;
                int winratePerc = (int) Math.Round(((double) league.Wins / (double) totalGames) * 100);
                soloqRanking =
                    $"{RiotService.GetStatStringFromRank(RiotService.GetRankFromPosition(league))} {league.Rank} ({league.LeaguePoints} LP) ({league.Wins}W / {league.Losses}L Winrate: {winratePerc}%)";
            }

            var thumbnail = RiftBot.GetService<RiotService>().GetSummonerIconUrlById(summoner.ProfileIconId);

            return StatEmbeds.SuccessEmbed(thumbnail, summoner, soloqRanking);
        }

        public async Task<Embed> GetUserStatAsync(ulong userId)
        {
            var statistics = await RiftBot.GetService<DatabaseService>().GetUserStatisticsAsync(userId);
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
                    await RiftBot.GetService<DatabaseService>().AddStatisticsAsync(userId, bragTotal: 1ul);
            }
            finally
            {
                bragMutex.Release();
            }

            await CheckAchievements(userId);

            return result;
        }

        static async Task<(BragResult, Embed)> GetUserBragInternalAsync(ulong userId)
        {
            (bool canBrag, ulong remainingSeconds) = await CanBrag(userId);

            if (!canBrag)
                return (BragResult.Error, BragEmbeds.CooldownEmbed(remainingSeconds));

            var dbSummoner = await RiftBot.GetService<DatabaseService>().GetUserLolDataAsync(userId);

            if (String.IsNullOrWhiteSpace(dbSummoner.AccountId))
                return (BragResult.Error, BragEmbeds.NoDataEmbed);

            (var matchlistResult, var matchlist) = await RiftBot
                                                         .GetService<RiotService>()
                                                         .GetLast20MatchesByAccountIdAsync(dbSummoner.SummonerRegion,
                                                                                           dbSummoner.AccountId);

            if (matchlistResult != RequestResult.Success)
                return (BragResult.Error, BragEmbeds.NoMatchesEmbed);

            (var matchDataResult, var matchData) = await RiftBot
                                                         .GetService<RiotService>()
                                                         .GetMatchById(dbSummoner.SummonerRegion,
                                                                       matchlist.Random().GameId);

            if (matchDataResult != RequestResult.Success)
            {
                RiftBot.Log.Error($"Failed to get match data");
                return (BragResult.Error, GenericEmbeds.ErrorEmbed);
            }

            long participantId = matchData.ParticipantIdentities
                                          .First(x => x.Player.CurrentAccountId == dbSummoner.SummonerId
                                                      || x.Player.AccountId == dbSummoner.SummonerId)
                                          .ParticipantId;

            var player = matchData.Participants
                                  .First(x => x.ParticipantId == participantId);

            if (player is null)
            {
                RiftBot.Log.Error($"Failed to get player object");
                return (BragResult.Error, GenericEmbeds.ErrorEmbed);
            }

            var champData = RiftBot.GetService<RiotService>().GetChampionById(player.ChampionId.ToString());

            if (champData is null)
            {
                RiftBot.Log.Error($"Failed to obtain champ data");
                return (BragResult.Error, GenericEmbeds.ErrorEmbed);
            }

            string champThumb = RiotService.GetChampionSquareByName(champData.Image);

            await RiftBot.GetService<DatabaseService>().SetLastBragTimestamp(userId, Helper.CurrentUnixTimestamp);

            var brag = new Brag(player.Stats.Win);
            await RiftBot.GetService<DatabaseService>().AddInventoryAsync(userId, coins: brag.Coins);

            var queue = RiftBot.GetService<RiotService>().GetQueueNameById(matchData.QueueId);
            return (BragResult.Success, BragEmbeds.SuccessEmbed(userId, champThumb, champData.Id, player, queue,
                                                                brag.Coins));
        }

        static async Task<(bool, ulong)> CanBrag(ulong userId)
        {
            var data = await RiftBot.GetService<DatabaseService>().GetUserLastBragTimestampAsync(userId);

            if (data.LastBragTimestamp == 0ul)
                return (true, 0ul);

            ulong diff = Helper.CurrentUnixTimestamp - data.LastBragTimestamp;

            bool result = diff > Settings.Economy.BragCooldownSeconds;

            return (result, result ? 0ul : Settings.Economy.BragCooldownSeconds - diff);
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

            RiftBot.Log.Info($"Opening chest for {userId}.");

            try
            {
                result = await OpenChestInternalAsync(userId, 1).ConfigureAwait(false);
                if (result.Item1 == OpenChestResult.Success)
                    await RiftBot.GetService<DatabaseService>().AddStatisticsAsync(userId, chestsOpenedTotal: 1);
            }
            finally
            {
                chestMutex.Release();
            }

            await CheckAchievements(userId);

            return result;
        }

        public async Task<(OpenChestResult, Embed)> OpenChestAllAsync(ulong userId)
        {
            await chestMutex.WaitAsync().ConfigureAwait(false);

            (OpenChestResult, Embed) result;

            RiftBot.Log.Info($"Opening all chests for {userId}");

            try
            {
                var dbInventory = await RiftBot.GetService<DatabaseService>().GetUserInventoryAsync(userId);
                result = await OpenChestInternalAsync(userId, dbInventory.Chests).ConfigureAwait(false);
                if (result.Item1 == OpenChestResult.Success)
                    await RiftBot.GetService<DatabaseService>()
                                 .AddStatisticsAsync(userId, chestsOpenedTotal: dbInventory.Chests);
            }
            finally
            {
                chestMutex.Release();
            }

            await CheckAchievements(userId);

            return result;
        }

        static async Task<(OpenChestResult, Embed)> OpenChestInternalAsync(ulong userId, uint amount)
        {
            var dbInventory = await RiftBot.GetService<DatabaseService>().GetUserInventoryAsync(userId);
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (dbInventory.Chests < amount || amount == 0)
                return (OpenChestResult.NoChests, ChestEmbeds.NoChestsEmbed);

            await RiftBot.GetService<DatabaseService>().RemoveInventoryAsync(userId, chests: amount);

            var chest = new Chest(userId, amount);

            await chest.GiveRewardAsync();

            await sgUser.SendEmbedAsync(ChestEmbeds.DMEmbed(chest.reward, amount));

            //var chatEmbed = ChestEmbeds.ChatEmbed(chest.reward, userId);
            //if (chatEmbed != null)
            //{
            //    if (!IonicClient.GetTextChannel(Settings.App.MainServerId, Settings.ChannelId.Chat, out var chatChannel))
            //        return (OpenChestResult.Error, GenericEmbeds.ErrorEmbed);
            //    await chatChannel.SendEmbedAsync(chatEmbed);
            //}

            return (OpenChestResult.Success, GenericEmbeds.EmptyEmbed);
        }

        public async Task<(OpenCapsuleResult, Embed)> OpenCapsuleAsync(ulong userId)
        {
            await capsuleMutex.WaitAsync().ConfigureAwait(false);

            (OpenCapsuleResult, Embed) result;

            RiftBot.Log.Info($"Opening capsule for {userId}.");

            try
            {
                result = await OpenCapsuleInternalAsync(userId).ConfigureAwait(false);
                if (result.Item1 == OpenCapsuleResult.Success)
                    await RiftBot.GetService<DatabaseService>().AddStatisticsAsync(userId, capsuleOpenedTotal: 1ul);
            }
            finally
            {
                capsuleMutex.Release();
            }

            await CheckAchievements(userId);

            return result;
        }

        static async Task<(OpenCapsuleResult, Embed)> OpenCapsuleInternalAsync(ulong userId)
        {
            var dbUserInventory = await RiftBot.GetService<DatabaseService>().GetUserInventoryAsync(userId);

            if (dbUserInventory.Capsules == 0u)
                return (OpenCapsuleResult.NoCapsules, CapsuleEmbeds.NoCapsulesEmbed);

            await RiftBot.GetService<DatabaseService>().RemoveInventoryAsync(userId, capsules: 1u);

            var capsule = new Capsule(userId);
            await capsule.GiveRewardsAsync(userId);

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
                return (OpenCapsuleResult.Error, GenericEmbeds.ErrorEmbed);

            await chatChannel.SendEmbedAsync(CapsuleEmbeds.ChatEmbed(userId, capsule));

            return (OpenCapsuleResult.Success, CapsuleEmbeds.DMEmbed(capsule));
        }

        public async Task<(OpenSphereResult, Embed)> OpenSphereAsync(ulong userId)
        {
            await sphereMutex.WaitAsync().ConfigureAwait(false);

            (OpenSphereResult, Embed) result;

            RiftBot.Log.Info($"Opening sphere for {userId}.");

            try
            {
                result = await OpenSphereInternalAsync(userId).ConfigureAwait(false);
                if (result.Item1 == OpenSphereResult.Success)
                    await RiftBot.GetService<DatabaseService>().AddStatisticsAsync(userId, sphereOpenedTotal: 1ul);
            }
            finally
            {
                sphereMutex.Release();
            }

            await CheckAchievements(userId);

            return result;
        }

        static async Task<(OpenSphereResult, Embed)> OpenSphereInternalAsync(ulong userId)
        {
            var dbInventory = await RiftBot.GetService<DatabaseService>().GetUserInventoryAsync(userId);

            if (dbInventory.Spheres == 0u)
                return (OpenSphereResult.NoSpheres, SphereEmbeds.NoSpheresEmbed);

            await RiftBot.GetService<DatabaseService>().RemoveInventoryAsync(userId, spheres: 1u);

            var sphere = new Sphere(userId);
            await sphere.GiveRewardsAsync(userId);

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
                return (OpenSphereResult.Error, GenericEmbeds.ErrorEmbed);

            await chatChannel.SendEmbedAsync(SphereEmbeds.ChatEmbed(userId, sphere));

            return (OpenSphereResult.Success, SphereEmbeds.DMEmbed(sphere));
        }

        public async Task<(StorePurchaseResult, Embed)> StorePurchaseAsync(ulong userId, StoreItem item)
        {
            await storeMutex.WaitAsync().ConfigureAwait(false);

            (StorePurchaseResult, Embed) result;

            RiftBot.Log.Info($"Store purchase: #{item.Id} by {userId}.");

            try
            {
                result = await StorePurchaseInternalAsync(userId, item).ConfigureAwait(false);
                if (result.Item1 == StorePurchaseResult.Success)
                    await RiftBot.GetService<DatabaseService>().AddStatisticsAsync(userId, purchasedItemsTotal: 1ul);
            }
            finally
            {
                storeMutex.Release();
            }

            await CheckAchievements(userId);

            return result;
        }

        static async Task<(StorePurchaseResult, Embed)> StorePurchaseInternalAsync(ulong userId, StoreItem item)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (sgUser is null)
                return (StorePurchaseResult.Error, GenericEmbeds.ErrorEmbed);

            (bool canBuy, ulong remainingSeconds) = await CanBuyStoreAsync(userId);

            if (!RiftBot.IsAdmin(sgUser) && !canBuy)
                return (StorePurchaseResult.Cooldown, StoreEmbeds.CooldownEmbed(remainingSeconds));

            (bool result, var currencyType) = await WithdrawCurrencyAsync();

            if (!result)
            {
                switch (currencyType)
                {
                    case Currency.Coins: return (StorePurchaseResult.NoCoins, StoreEmbeds.NoCoinsEmbed);

                    case Currency.Tokens: return (StorePurchaseResult.NoTokens, StoreEmbeds.NoTokensEmbed);
                }
            }

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var channel))
                return (StorePurchaseResult.Error, GenericEmbeds.ErrorEmbed);

            switch (item.Type)
            {
                case StoreItemType.DoubleExp:
                    await RiftBot.GetService<DatabaseService>().AddInventoryAsync(userId, doubleExps: 1u);
                    break;

                case StoreItemType.Capsule:
                    await RiftBot.GetService<DatabaseService>().AddInventoryAsync(userId, capsules: 1u);
                    break;

                case StoreItemType.UsualTicket:
                    await RiftBot.GetService<DatabaseService>().AddInventoryAsync(userId, usualTickets: 1u);
                    break;

                case StoreItemType.RareTicket:
                    await RiftBot.GetService<DatabaseService>().AddInventoryAsync(userId, rareTickets: 1u);
                    break;

                case StoreItemType.Chest:
                    await RiftBot.GetService<DatabaseService>().AddInventoryAsync(userId, chests: 1u);
                    break;

                case StoreItemType.Token:
                    await RiftBot.GetService<DatabaseService>().AddInventoryAsync(userId, tokens: 1u);
                    break;

                case StoreItemType.Sphere:
                    await RiftBot.GetService<DatabaseService>().AddInventoryAsync(userId, spheres: 1u);
                    break;

                case StoreItemType.BotRespect:
                    await RiftBot.GetService<DatabaseService>().AddInventoryAsync(userId, respects: 1u);
                    break;

                case StoreItemType.TempRole:
                    var role = new TempRole(userId, item.RoleId, TimeSpan.FromDays(30u));
                    await RiftBot.GetService<RoleService>().AddTempRoleAsync(role);
                    await channel.SendEmbedAsync(StoreEmbeds.ChatEmbed(sgUser, item));
                    break;

                case StoreItemType.PermanentRole:
                    await channel.SendEmbedAsync(StoreEmbeds.ChatEmbed(sgUser, item));
                    await RiftBot.GetService<RoleService>().AddRoleAsync(userId, item.RoleId);
                    break;
            }

            string balance = await GetBalanceString();

            async Task<(bool, Currency)> WithdrawCurrencyAsync()
            {
                var dbInventory = await RiftBot.GetService<DatabaseService>().GetUserInventoryAsync(userId);

                switch (item.Currency)
                {
                    case Currency.Coins:
                    {
                        if (dbInventory.Coins < item.Price)
                            return (false, item.Currency);

                        await RiftBot.GetService<DatabaseService>().RemoveInventoryAsync(userId, coins: item.Price);
                        break;
                    }

                    case Currency.Tokens:
                    {
                        if (dbInventory.Tokens < item.Price)
                            return (false, item.Currency);

                        await RiftBot.GetService<DatabaseService>().RemoveInventoryAsync(userId, tokens: item.Price);
                        break;
                    }
                }

                return (true, item.Currency);
            }

            await RiftBot.GetService<DatabaseService>().SetLastStoreTimestampAsync(userId, Helper.CurrentUnixTimestamp);

            async Task<string> GetBalanceString()
            {
                var dbInventory = await RiftBot.GetService<DatabaseService>().GetUserInventoryAsync(userId);

                return $"{Settings.Emote.Coin} {dbInventory.Coins} {Settings.Emote.Token} {dbInventory.Tokens}";
            }

            return (StorePurchaseResult.Success, StoreEmbeds.DMSuccessEmbed(item, balance));
        }

        static async Task<(bool, ulong)> CanBuyStoreAsync(ulong userId)
        {
            var dbStore = await RiftBot.GetService<DatabaseService>().GetUserLastStoreTimestampAsync(userId);

            if (dbStore.LastStoreTimestamp == 0ul)
                return (true, ulong.MinValue);

            ulong diff = Helper.CurrentUnixTimestamp - dbStore.LastStoreTimestamp;

            return (diff > Settings.Economy.StoreCooldownSeconds, Settings.Economy.StoreCooldownSeconds - diff);
        }

        public async Task<(GiftResult, Embed)> GiftAsync(SocketGuildUser fromUser, SocketGuildUser toUser, uint type)
        {
            await giftMutex.WaitAsync().ConfigureAwait(false);

            (GiftResult, Embed) result;

            RiftBot.Log.Info($"Gift from {fromUser.Id} to {toUser.Id}.");

            try
            {
                result = await GiftInternalAsync(fromUser, toUser, type).ConfigureAwait(false);

                if (result.Item1 == GiftResult.Success)
                {
                    await RiftBot.GetService<DatabaseService>().AddStatisticsAsync(fromUser.Id, giftsSent: 1ul);
                    await RiftBot.GetService<DatabaseService>().AddStatisticsAsync(toUser.Id, giftsReceived: 1ul);

                    bool giftToModer = RiftBot.IsModerator(toUser);
                    bool giftToBotKeeper = RiftBot.IsDeveloper(toUser);

                    if (giftToModer || giftToBotKeeper)
                    {
                        var achievements =
                            await RiftBot.GetService<DatabaseService>().GetUserAchievementsAsync(fromUser.Id);

                        if (!achievements.GiftToBotKeeper && giftToBotKeeper)
                        {
                            await AddAchievement(fromUser.Id, "умный ход", Achievements.GiftToBotKeeper);
                            await RiftBot.GetService<DatabaseService>()
                                         .UpdateAchievementAsync(fromUser.Id, giftToBotKeeper: true);
                        }

                        if (!achievements.GiftToModerator && giftToModer)
                        {
                            await AddAchievement(fromUser.Id, "покажи себя", Achievements.GiftToModerator);
                            await RiftBot.GetService<DatabaseService>()
                                         .UpdateAchievementAsync(fromUser.Id, giftToModerator: true);
                        }
                    }
                }
            }
            finally
            {
                giftMutex.Release();
            }

            await CheckAchievements(fromUser.Id);

            return result;
        }

        static async Task<(GiftResult, Embed)> GiftInternalAsync(SocketGuildUser fromUser, SocketGuildUser toUser,
                                                                 uint type)
        {
            if (fromUser.Id == toUser.Id)
            {
                RiftBot.Log.Debug($"[Gift] Ouch, self-gift.");
                return (GiftResult.SelfGift, GiftEmbeds.SelfGiftEmbed);
            }

            (bool canGift, ulong remainingSeconds) = await CanGift(fromUser.Id);

            if (!RiftBot.IsAdmin(fromUser) && !canGift)
                return (GiftResult.Cooldown, GiftEmbeds.CooldownEmbed(remainingSeconds));

            var giftItem = Gift.GetGiftItemById(type);

            if (giftItem is null)
                return (GiftResult.NoItem, GiftEmbeds.IncorrectNumberEmbed);

            (bool result, var currencyType) = await WithdrawCurrencyAsync();

            if (!result)
            {
                switch (currencyType)
                {
                    case Currency.Coins: return (GiftResult.NoCoins, GiftEmbeds.NoCoinsEmbed);

                    case Currency.Tokens: return (GiftResult.NoTokens, GiftEmbeds.NoTokensEmbed);
                }
            }

            await RiftBot.GetService<DatabaseService>().SetLastGiftTimestamp(fromUser.Id, Helper.CurrentUnixTimestamp);

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
                return (GiftResult.Error, GenericEmbeds.ErrorEmbed);

            string giftString = "";
            switch (giftItem.Type)
            {
                case GiftItemType.CoinsRandom:
                    uint coins = Helper.NextUInt(1000, 3501);
                    await RiftBot.GetService<DatabaseService>().AddInventoryAsync(toUser.Id, coins: coins);
                    giftString = $"{Settings.Emote.Coin} {coins}";
                    break;

                case GiftItemType.ChestsRandom:
                    uint chests = Helper.NextUInt(4, 12);
                    await RiftBot.GetService<DatabaseService>().AddInventoryAsync(toUser.Id, chests: chests);
                    giftString = $"{Settings.Emote.Chest} {chests}";
                    break;

                case GiftItemType.Chest:
                    await RiftBot.GetService<DatabaseService>().AddInventoryAsync(toUser.Id, chests: 1);
                    giftString = $"{Settings.Emote.Chest} 1";
                    break;

                case GiftItemType.Sphere:
                    await RiftBot.GetService<DatabaseService>().AddInventoryAsync(toUser.Id, spheres: 1u);
                    giftString = $"{Settings.Emote.Sphere} сферу";
                    break;

                case GiftItemType.UsualTicket:
                    await RiftBot.GetService<DatabaseService>().AddInventoryAsync(toUser.Id, usualTickets: 1u);
                    giftString = $"{Settings.Emote.Ctickets} обычный билет";
                    break;

                case GiftItemType.BotRespect:
                    await RiftBot.GetService<DatabaseService>().AddInventoryAsync(toUser.Id, respects: 1);
                    giftString = $"{Settings.Emote.BotRespect} уважение ботов";
                    break;

                case GiftItemType.RareTicket:
                    await RiftBot.GetService<DatabaseService>().AddInventoryAsync(toUser.Id, rareTickets: 1u);
                    giftString = $"{Settings.Emote.Gtickets} редкий билет";
                    break;

                case GiftItemType.DoubleExp:
                    await RiftBot.GetService<DatabaseService>().AddInventoryAsync(toUser.Id, doubleExps: 1u);
                    giftString = $"{Settings.Emote.PowerupDoubleExperience} двойной опыт";
                    break;
            }

            var dbInventory = await RiftBot.GetService<DatabaseService>().GetUserInventoryAsync(fromUser.Id);

            await fromUser.SendEmbedAsync(GiftEmbeds.DMFromEmbed(toUser, dbInventory));
            await toUser.SendEmbedAsync(GiftEmbeds.DMToEmbed(fromUser, giftString));

            var msg = await chatChannel.SendEmbedAsync(GiftEmbeds.ChatEmbed(fromUser, toUser, giftString));
            RiftBot.GetService<MessageService>().TryAddDelete(new DeleteMessage(msg, TimeSpan.FromMinutes(3)));

            RiftBot.Log.Debug($"[Gift] Success.");

            return (GiftResult.Success, GenericEmbeds.EmptyEmbed);

            async Task<(bool, Currency)> WithdrawCurrencyAsync()
            {
                var dbInventory2 = await RiftBot.GetService<DatabaseService>().GetUserInventoryAsync(fromUser.Id);

                switch (giftItem.Currency)
                {
                    case Currency.Coins:
                    {
                        if (dbInventory2.Coins < giftItem.Price)
                            return (false, giftItem.Currency);

                        await RiftBot.GetService<DatabaseService>()
                                     .RemoveInventoryAsync(fromUser.Id, coins: giftItem.Price);
                        break;
                    }

                    case Currency.Tokens:
                    {
                        if (dbInventory2.Tokens < giftItem.Price)
                            return (false, giftItem.Currency);

                        await RiftBot.GetService<DatabaseService>()
                                     .RemoveInventoryAsync(fromUser.Id, tokens: giftItem.Price);
                        break;
                    }
                }

                return (true, giftItem.Currency);
            }
        }

        static async Task<(bool, ulong)> CanGift(ulong userId)
        {
            var data = await RiftBot.GetService<DatabaseService>().GetUserLastGiftTimestampAsync(userId);

            if (data.LastGiftTimestamp == 0ul)
                return (true, 0ul);

            ulong diff = Helper.CurrentUnixTimestamp - data.LastGiftTimestamp;

            bool result = diff > Settings.Economy.GiftCooldownSeconds;

            return (result, result ? 0ul : Settings.Economy.GiftCooldownSeconds - diff);
        }

        public async Task<(AttackResult, Embed)> AttackAsync(SocketGuildUser sgAttacker, SocketGuildUser sgTarget)
        {
            await attackMutex.WaitAsync().ConfigureAwait(false);

            (AttackResult, Embed) result;

            RiftBot.Log.Info($"Attacktime! {sgAttacker.Id} want to kick {sgTarget.Id}'s ass.");

            try
            {
                result = await AttackInternalAsync(sgAttacker, sgTarget).ConfigureAwait(false);
                if (result.Item1 == AttackResult.Success)
                {
                    await RiftBot.GetService<DatabaseService>().AddStatisticsAsync(sgAttacker.Id, attacksDone: 1ul);
                    await RiftBot.GetService<DatabaseService>().AddStatisticsAsync(sgTarget.Id, attacksReceived: 1ul);

                    if (sgTarget.Id == 178443743026872321ul
                        && IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat,
                                                      out var chatChannel))
                    {
                        var achivements =
                            await RiftBot.GetService<DatabaseService>().GetUserAchievementsAsync(sgAttacker.Id);

                        if (!achivements.AttackWise)
                        {
                            await AddAchievement(sgAttacker.Id, "разоритель основателя", Achievements.AttackWise);
                            await RiftBot.GetService<DatabaseService>()
                                         .UpdateAchievementAsync(sgAttacker.Id, attackWise: true);
                        }
                    }
                }
            }
            finally
            {
                attackMutex.Release();
            }

            await CheckAchievements(sgAttacker.Id);
            await CheckAchievements(sgTarget.Id);

            return result;
        }

        static async Task<(AttackResult, Embed)> AttackInternalAsync(SocketGuildUser sgAttacker,
                                                                     SocketGuildUser sgTarget)
        {
            if (sgAttacker.Id == sgTarget.Id)
                return (AttackResult.SelfAttack, AttackEmbeds.SelfAttackEmbed);

            //if (IonicBot.IsModerator(sgTarget))
            //	return (AttackResult.TargetModerator, embedAttackModerator);

            //if (sgTarget.Id == 178443743026872321ul)
            //	return (AttackResult.TargetWise, embedAttackWise);

            if (sgTarget.Id == 212997107525746690ul || sgTarget.Id == 393440346702610433ul)
                return (AttackResult.TargetIon, AttackEmbeds.BotKeeperEmbed);

            (bool canBeAttacked, ulong remainingSeconds) = await CanBeAttackedAgain(sgTarget.Id);

            if (!canBeAttacked)
                return (AttackResult.TooOftenSame, AttackEmbeds.TargetCooldownEmbed(remainingSeconds));

            var attackerDbProfile = await RiftBot.GetService<DatabaseService>().GetUserProfileAsync(sgAttacker.Id);
            var attackerDbInventory = await RiftBot.GetService<DatabaseService>().GetUserInventoryAsync(sgAttacker.Id);

            if (attackerDbProfile.Level < Settings.Economy.AttackMinimumLevel)
                return (AttackResult.LowUserLevel, AttackEmbeds.LowLevelEmbed);

            var canAttackResult = await CanAttackAsync(sgAttacker.Id);

            if (!canAttackResult.Item1)
                return (AttackResult.OnUserCooldown, AttackEmbeds.CooldownEmbed(canAttackResult.Item2));

            var targetDbUserProfile = await RiftBot.GetService<DatabaseService>().GetUserProfileAsync(sgTarget.Id);
            var targetDbUserInventory = await RiftBot.GetService<DatabaseService>().GetUserInventoryAsync(sgTarget.Id);

            if (targetDbUserProfile.Level < Settings.Economy.AttackMinimumLevel)
                return (AttackResult.LowTargetLevel, AttackEmbeds.LowTargetLevelEmbed);

            if (attackerDbInventory.Coins < Settings.Economy.AttackPrice)
                return (AttackResult.NoCoins, AttackEmbeds.NoCoinsEmbed);

            await RiftBot.GetService<DatabaseService>()
                         .RemoveInventoryAsync(sgAttacker.Id, coins: Settings.Economy.AttackPrice);

            var attack = new Attack(sgAttacker.Id, sgTarget.Id, targetDbUserInventory.Coins,
                                    targetDbUserInventory.Chests);

            switch (attack.Loot)
            {
                case AttackLoot.Coins:
                {
                    if (targetDbUserInventory.Coins < attack.Count)
                        attack.Count = targetDbUserInventory.Coins; //stealing target's last monies :(

                    await RiftBot.GetService<DatabaseService>().RemoveInventoryAsync(sgTarget.Id, coins: attack.Count);
                    await RiftBot.GetService<DatabaseService>().AddInventoryAsync(sgAttacker.Id, coins: attack.Count);
                    break;
                }
                case AttackLoot.Chests:
                {
                    if (targetDbUserInventory.Chests < attack.Count)
                        attack.Count = targetDbUserInventory.Chests; //stealing target's last chests :(

                    await RiftBot.GetService<DatabaseService>().RemoveInventoryAsync(sgTarget.Id, chests: attack.Count);
                    await RiftBot.GetService<DatabaseService>().AddInventoryAsync(sgAttacker.Id, chests: attack.Count);
                    break;
                }
                case AttackLoot.Mute:
                {
                    var role = new TempRole(sgTarget.Id, Settings.RoleId.Attacked, TimeSpan.FromMinutes(attack.Count));
                    await RiftBot.GetService<RoleService>().AddTempRoleAsync(role);
                    break;
                }
                case AttackLoot.ReversedMute:
                {
                    var role = new TempRole(sgAttacker.Id, Settings.RoleId.Attacked,
                                            TimeSpan.FromMinutes(attack.Count));
                    await RiftBot.GetService<RoleService>().AddTempRoleAsync(role);
                    break;
                }
                case AttackLoot.MutualMute:
                {
                    var role1 = new TempRole(sgTarget.Id, Settings.RoleId.Attacked, TimeSpan.FromMinutes(attack.Count));
                    var role2 = new TempRole(sgAttacker.Id, Settings.RoleId.Attacked,
                                             TimeSpan.FromMinutes(attack.Count));
                    await RiftBot.GetService<RoleService>().AddTempRoleAsync(role1);
                    await RiftBot.GetService<RoleService>().AddTempRoleAsync(role2);
                    break;
                }
            }

            await UpdateLastBeingAttackedSameCooldown(sgTarget.Id);

            await sgAttacker.SendEmbedAsync(AttackEmbeds.AttackerEmbed(sgTarget, attack));
            await sgTarget.SendEmbedAsync(AttackEmbeds.TargetEmbed(sgAttacker, attack));

            await RiftBot.GetService<DatabaseService>()
                         .SetLastAttackTimestampAsync(sgAttacker.Id, Helper.CurrentUnixTimestamp); //attacker cd

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
                return (AttackResult.Error, GenericEmbeds.ErrorEmbed);

            var msg = await chatChannel.SendEmbedAsync(AttackEmbeds.ChatEmbed(sgAttacker, sgTarget));
            RiftBot.GetService<MessageService>().TryAddDelete(new DeleteMessage(msg, TimeSpan.FromMinutes(3)));

            msg = await chatChannel.SendEmbedAsync(AttackEmbeds.AttackDescEmbed(sgAttacker, sgTarget, attack));
            RiftBot.GetService<MessageService>().TryAddDelete(new DeleteMessage(msg, TimeSpan.FromMinutes(3)));

            return (AttackResult.Success, GenericEmbeds.EmptyEmbed);
        }

        static async Task<(bool, ulong)> CanAttackAsync(ulong userId)
        {
            var dbAttack = await RiftBot.GetService<DatabaseService>().GetUserLastAttackTimestampAsync(userId);

            if (dbAttack.LastAttackTimestamp == 0ul)
                return (true, ulong.MinValue);

            ulong diff = Helper.CurrentUnixTimestamp - dbAttack.LastAttackTimestamp;

            return (diff > Settings.Economy.AttackPerUserCooldownSeconds,
                    Settings.Economy.AttackPerUserCooldownSeconds - diff);
        }

        static async Task<(bool, ulong)> CanBeAttackedAgain(ulong userId)
        {
            var dbAttacked = await RiftBot.GetService<DatabaseService>().GetUserLastBeingAttackedTimestampAsync(userId);

            if (dbAttacked.LastBeingAttackedTimestamp == 0ul) // no cd
                return (true, dbAttacked.LastBeingAttackedTimestamp);

            ulong diff = Helper.CurrentUnixTimestamp - dbAttacked.LastBeingAttackedTimestamp;

            bool result = diff > Settings.Economy.AttackSameUserCooldownSeconds;

            return (result, Settings.Economy.AttackSameUserCooldownSeconds - diff);
        }

        static async Task UpdateLastBeingAttackedSameCooldown(ulong userId)
        {
            await RiftBot.GetService<DatabaseService>()
                         .SetLastBeingAttackedTimestampAsync(userId, Helper.CurrentUnixTimestamp);
        }

        public async Task<GiftResult> GiftSpecialAsync(ulong fromId, ulong toId, GiftSource giftSource)
        {
            var user = IonicClient.GetGuildUserById(Settings.App.MainGuildId, toId);
            var chatEmbed = new EmbedBuilder();
            var dmEmbed = new EmbedBuilder();

            switch (giftSource)
            {
                case GiftSource.Streamer:

                    await RiftBot.GetService<DatabaseService>().AddInventoryAsync(toId, coins: 300u, chests: 2u);

                    chatEmbed
                        .WithDescription($"Стример <@{fromId}> подарил {Settings.Emote.Coin} 300 {Settings.Emote.Chest} 2 призывателю <@{toId}>");
                    break;

                case GiftSource.Moderator:

                    await RiftBot.GetService<DatabaseService>().AddInventoryAsync(toId, coins: 100u, chests: 1u);

                    chatEmbed
                        .WithDescription($"Призыватель <@{toId}> получил {Settings.Emote.Coin} 100 {Settings.Emote.Chest} 1");
                    break;

                case GiftSource.Voice:

                    var coins = Helper.NextUInt(50, 350);
                    await RiftBot.GetService<DatabaseService>().AddInventoryAsync(toId, coins: coins);

                    chatEmbed
                        .WithAuthor("Голосовые каналы", Settings.Emote.VoiceUrl)
                        .WithDescription($"Призыватель <@{toId}> получил {Settings.Emote.Coin} {coins} за активность.");

                    dmEmbed
                        .WithAuthor("Голосовые каналы", Settings.Emote.VoiceUrl)
                        .WithDescription($"Вы получили {Settings.Emote.Coin} {coins} за активность.");
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
            var dbInventory = await RiftBot.GetService<DatabaseService>().GetUserInventoryAsync(userId);

            if (dbInventory.PowerupsDoubleExperience == 0)
            {
                await sgUser.SendEmbedAsync(ActivateEmbeds.NoSuchPowerupEmbed);
                return;
            }

            var dbLevel = await RiftBot.GetService<DatabaseService>().GetUserLevelAsync(userId);
            if (dbLevel.DoubleExpTimestamp > Helper.CurrentUnixTimestamp)
            {
                await sgUser.SendEmbedAsync(ActivateEmbeds.DoubleExpNotExpiredEmbed);
                return;
            }

            await RiftBot.GetService<DatabaseService>().RemoveInventoryAsync(userId, doubleExps: 1);

            var dateTime = DateTime.Now.AddHours(12.0);
            await RiftBot.GetService<DatabaseService>().SetDoubleExpTimestampAsync(userId, dateTime.ToUnixTimestamp());

            await sgUser.SendEmbedAsync(ActivateEmbeds.DoubleExpSuccessEmbed);

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
                return;

            await chatChannel.SendEmbedAsync(ActivateEmbeds.DoubleExpChatEmbed(sgUser));
        }

        public async Task ActivateBotRespect(ulong userId)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);
            var dbInventory = await RiftBot.GetService<DatabaseService>().GetUserInventoryAsync(userId);

            if (dbInventory.PowerupsBotRespect == 0)
            {
                await sgUser.SendEmbedAsync(ActivateEmbeds.NoSuchPowerupEmbed);
                return;
            }

            var dbuser = await RiftBot.GetService<DatabaseService>().GetUserProfileAsync(userId);
            if (dbuser.BotRespectTimestamp > Helper.CurrentUnixTimestamp)
            {
                await sgUser.SendEmbedAsync(ActivateEmbeds.BotRespectNotExpiredEmbed);
                return;
            }

            await RiftBot.GetService<DatabaseService>().RemoveInventoryAsync(userId, respects: 1);

            var dateTime = DateTime.Now.AddHours(12.0);
            await RiftBot.GetService<DatabaseService>().SetBotRespeсtTimestampAsync(userId, dateTime.ToUnixTimestamp());

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
        Cooldown,
        SelfGift,
        NoCoins,
        NoTokens,
        NoItem,
        HasBotRespect,
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
