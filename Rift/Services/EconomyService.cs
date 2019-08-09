using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Settings = Rift.Configuration.Settings;

using Rift.Database;
using Rift.Services.Message;
using Rift.Services.Reward;
using Rift.Util;

using Discord;

using IonicLib;

namespace Rift.Services
{
    public class EconomyService
    {
        static Timer ratingUpdateTimer;
        static readonly TimeSpan RatingTimerCooldown = TimeSpan.FromMinutes(10);

        public void Init()
        {
            ratingUpdateTimer = new Timer(
                async delegate
                {
                    await UpdateRatingAsync();
                },
                null,
                TimeSpan.FromMinutes(5),
                RatingTimerCooldown);
        }

        public static List<ulong> SortedRating { get; private set; }

        public static async Task ShowActiveUsersAsync()
        {
            var topTen = await DB.Users.GetTopTenByExpAsync(x =>
                !(IonicClient.GetGuildUserById(Settings.App.MainGuildId, x.UserId) is null));

            if (topTen.Count == 0)
                return;

            await RiftBot.SendMessageAsync("economy-activeusers", Settings.ChannelId.Commands,
                new FormatData(IonicClient.Client.CurrentUser.Id)
                {
                    Economy = new EconomyData
                    {
                        Top10Exp = topTen
                    }
                });
        }

        public static async Task ShowRichUsersAsync()
        {
            var topTen = await DB.Users.GetTopTenByCoinsAsync(x =>
                !(IonicClient.GetGuildUserById(Settings.App.MainGuildId, x.UserId) is null));

            if (topTen.Count == 0)
                return;

            await RiftBot.SendMessageAsync("economy-richusers", Settings.ChannelId.Commands,
                new FormatData(IonicClient.Client.CurrentUser.Id)
                {
                    Economy = new EconomyData
                    {
                        Top10Coins = topTen
                    }
                });
        }

        public static async Task ProcessMessageAsync(IUserMessage message)
        {
            await AddExpAsync(message.Author.Id, 1u).ConfigureAwait(false);
        }

        static async Task AddExpAsync(ulong userId, uint exp)
        {
            await DB.Users.AddExperienceAsync(userId, exp)
                    .ContinueWith(async _ => await CheckLevelUpAsync(userId).ConfigureAwait(false));
        }

        static async Task CheckLevelUpAsync(ulong userId)
        {
            var dbUser = await DB.Users.GetAsync(userId);

            if (dbUser.Experience != uint.MaxValue)
            {
                var newLevel = dbUser.Level + 1u;

                while (dbUser.Experience >= GetExpForLevel(newLevel)) newLevel++;

                newLevel--;

                if (newLevel > dbUser.Level)
                {
                    await DB.Users.SetLevelAsync(dbUser.UserId, newLevel);

                    RiftBot.Log.Info($"{dbUser.UserId.ToString()} just leveled up: " +
                                     $"{dbUser.Level.ToString()} => {newLevel.ToString()}");

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
            
            for (var level = fromLevel + 1; level <= toLevel; level++)
            {
                if (level == 100u)
                    reward.AddCapsules(1u);
                else if (level == 50u)
                    reward.AddSpheres(1u);
                else if (level % 25u == 0u)
                    reward.AddSpheres(1u);
                else if (level % 10u == 0u)
                    reward.AddTokens(2u);
                else if (level % 5u == 0u)
                    reward.AddTickets(1u).AddCoins(2_000u);
                else
                    reward.AddChests(1u).AddCoins(2_000u);

                var nitroBooster = await DB.Roles.GetAsync(91);
                if (IonicClient.HasRolesAny(sgUser, nitroBooster.RoleId))
                    reward.AddChests(2u);
                
                var rankGold = await DB.Roles.GetAsync(3);
                if (IonicClient.HasRolesAny(sgUser, rankGold.RoleId))
                    reward.AddCoins(250u);
                
                var rankPlatinum = await DB.Roles.GetAsync(11);
                if (IonicClient.HasRolesAny(sgUser, rankPlatinum.RoleId))
                    reward.AddCoins(500u);
                
                var rankDiamond = await DB.Roles.GetAsync(8);
                if (IonicClient.HasRolesAny(sgUser, rankDiamond.RoleId))
                    reward.AddCoins(750u);
                
                var rankMaster = await DB.Roles.GetAsync(79);
                if (IonicClient.HasRolesAny(sgUser, rankMaster.RoleId))
                    reward.AddCoins(1000u);
                
                var rankGrandmaster = await DB.Roles.GetAsync(71);
                if (IonicClient.HasRolesAny(sgUser, rankGrandmaster.RoleId))
                    reward.AddCoins(1250u);
                
                var rankChallenger = await DB.Roles.GetAsync(23);
                if (IonicClient.HasRolesAny(sgUser, rankChallenger.RoleId))
                    reward.AddCoins(1500u);
            }

            await reward.DeliverToAsync(userId);
            await RiftBot.SendMessageAsync("levelup", Settings.ChannelId.Chat, new FormatData(userId)
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
            SortedRating = await DB.Users.GetAllSortedAsync();
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
