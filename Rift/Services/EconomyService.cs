using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Settings = Rift.Configuration.Settings;

using Rift.Services.Interfaces;
using Rift.Services.Message;
using Rift.Services.Reward;

using IonicLib;

namespace Rift.Services
{
    public class EconomyService : IEconomyService
    {
        static Timer ratingUpdateTimer;
        static readonly TimeSpan RatingTimerCooldown = TimeSpan.FromMinutes(10);
        static readonly Func<uint, uint> ExpFormula = level => (uint) (Math.Pow(level, 1.5) * 40 - 40);

        readonly IMessageService messageService;
        readonly IRewardService rewardService;

        public EconomyService(IMessageService messageService,
                              IRewardService rewardService)
        {
            this.messageService = messageService;
            this.rewardService = rewardService;
        }

        public void Init()
        {
            ratingUpdateTimer = new Timer(
                async delegate { await UpdateRatingAsync(); },
                null,
                TimeSpan.FromMinutes(5),
                RatingTimerCooldown);
        }

        public List<ulong> SortedRating { get; set; }

        public async Task ShowActiveUsersAsync()
        {
            var topTen = await DB.Users.GetTopTenByExpAsync(x => x.UserId != 0ul);

            if (topTen.Count == 0)
                return;

            await messageService.SendMessageAsync("economy-activeusers", Settings.ChannelId.Commands,
                new FormatData(IonicHelper.Client.CurrentUser.Id)
                {
                    Economy = new EconomyData
                    {
                        Top10Exp = topTen
                    }
                });
        }

        public async Task ShowRichUsersAsync()
        {
            var topTen = await DB.Users.GetTopTenByCoinsAsync(x => x.UserId != 0ul);

            if (topTen.Count == 0)
                return;

            await messageService.SendMessageAsync("economy-richusers", Settings.ChannelId.Commands,
                new FormatData(IonicHelper.Client.CurrentUser.Id)
                {
                    Economy = new EconomyData
                    {
                        Top10Coins = topTen
                    }
                });
        }

        public async Task ProcessMessageAsync(ulong userId)
        {
            await AddExpAsync(userId, 1u).ConfigureAwait(false);
        }

        async Task AddExpAsync(ulong userId, uint exp)
        {
            await DB.Users.AddExperienceAsync(userId, exp)
                .ContinueWith(async _ => await CheckLevelUpAsync(userId).ConfigureAwait(false));
        }

        async Task CheckLevelUpAsync(ulong userId)
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

                    RiftBot.Log.Information($"{dbUser.UserId.ToString()} just leveled up: " +
                                            $"{dbUser.Level.ToString()} => {newLevel.ToString()}");

                    if (newLevel == 1u)
                        return; //no rewards on level 1

                    await GiveRewardsForLevelAsync(dbUser.UserId, dbUser.Level, newLevel);
                }
            }
        }

        async Task GiveRewardsForLevelAsync(ulong userId, uint fromLevel, uint toLevel)
        {
            if (!IonicHelper.GetGuildUserById(Settings.App.MainGuildId, userId, out var sgUser))
                return;

            var reward = new ItemReward();

            for (var level = fromLevel + 1; level <= toLevel; level++)
            {
                if (level == 100u)
                    reward.AddCapsules(1u).AddCoins(2_000u);
                else if (level == 50u)
                    reward.AddSpheres(1u).AddCoins(2_000u);
                else if (level % 25u == 0u)
                    reward.AddSpheres(1u).AddCoins(2_000u);
                else if (level % 10u == 0u)
                    reward.AddTokens(2u).AddCoins(2_000u);
                else if (level % 5u == 0u)
                    reward.AddTickets(1u).AddCoins(2_000u);
                else
                    reward.AddChests(1u).AddCoins(2_000u);

                var nitroBooster = await DB.Roles.GetAsync(91);
                if (IonicHelper.HasRolesAny(sgUser, nitroBooster.RoleId))
                    reward.AddChests(2u);

                var rankGold = await DB.Roles.GetAsync(3);
                if (IonicHelper.HasRolesAny(sgUser, rankGold.RoleId))
                    reward.AddCoins(250u);

                var rankPlatinum = await DB.Roles.GetAsync(11);
                if (IonicHelper.HasRolesAny(sgUser, rankPlatinum.RoleId))
                    reward.AddCoins(500u);

                var rankDiamond = await DB.Roles.GetAsync(8);
                if (IonicHelper.HasRolesAny(sgUser, rankDiamond.RoleId))
                    reward.AddCoins(750u);

                var rankMaster = await DB.Roles.GetAsync(79);
                if (IonicHelper.HasRolesAny(sgUser, rankMaster.RoleId))
                    reward.AddCoins(1000u);

                var rankGrandmaster = await DB.Roles.GetAsync(71);
                if (IonicHelper.HasRolesAny(sgUser, rankGrandmaster.RoleId))
                    reward.AddCoins(1250u);

                var rankChallenger = await DB.Roles.GetAsync(23);
                if (IonicHelper.HasRolesAny(sgUser, rankChallenger.RoleId))
                    reward.AddCoins(1500u);
            }

            await rewardService.DeliverToAsync(userId, reward);
            await messageService.SendMessageAsync("levelup", Settings.ChannelId.Chat, new FormatData(userId)
            {
                Reward = reward
            });
        }

        public async Task GetUserCooldownsAsync(ulong userId)
        {
            await messageService.SendMessageAsync("user-cooldowns", Settings.ChannelId.Commands, new FormatData(userId));
        }

        public async Task GetUserProfileAsync(ulong userId)
        {
            await messageService.SendMessageAsync("user-profile", Settings.ChannelId.Commands, new FormatData(userId));
        }

        public async Task GetUserStatAsync(ulong userId)
        {
            var statistics = await DB.Statistics.GetAsync(userId);

            await messageService.SendMessageAsync("user-stat", Settings.ChannelId.Commands, new FormatData(userId)
            {
                Statistics = statistics
            });
        }

        public async Task GetQuests(ulong userId)
        {
            await messageService.SendMessageAsync("user-quests", Settings.ChannelId.Commands, new FormatData(userId));
        }

        async Task UpdateRatingAsync()
        {
            SortedRating = await DB.Users.GetAllSortedAsync();
        }

        public uint GetExpForLevel(uint level)
        {
            return ExpFormula.Invoke(level);
        }
    }

    public enum Currency
    {
        Coins,
        Tokens,
    }
}
