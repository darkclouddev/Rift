using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Discord;

using Settings = Rift.Configuration.Settings;

using Rift.Database;
using Rift.Events;
using Rift.Services.Message;
using Rift.Services.Reward;

using IonicLib.Extensions;
using IonicLib.Util;

namespace Rift.Services
{
    public class BragService
    {
        public static EventHandler<BragEventArgs> OnUserBrag;

        static readonly SemaphoreSlim Mutex = new SemaphoreSlim(1);

        public async Task GetUserBragAsync(ulong userId)
        {
            await Mutex.WaitAsync().ConfigureAwait(false);

            try
            {
                await GetUserBragInternalAsync(userId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                RiftBot.Log.Error("Failed to brag:");
                RiftBot.Log.Error(ex);
                return;
            }
            finally
            {
                Mutex.Release();
            }
        }

        static async Task GetUserBragInternalAsync(ulong userId)
        {
            if (!await CanBrag(userId))
            {
                await RiftBot.SendMessageAsync("brag-cooldown", Settings.ChannelId.Commands, new FormatData(userId));
                return;
            }

            var dbSummoner = await DB.LeagueData.GetAsync(userId);

            if (dbSummoner is null || string.IsNullOrWhiteSpace(dbSummoner.AccountId))
            {
                await RiftBot.SendMessageAsync("loldata-nodata", Settings.ChannelId.Commands, new FormatData(userId));
                return;
            }

            (var matchlistResult, var matchlist) = await RiftBot.GetService<RiotService>()
                .GetLast20MatchesByAccountIdAsync(dbSummoner.SummonerRegion, dbSummoner.AccountId);

            if (matchlistResult != RequestResult.Success)
            {
                await RiftBot.SendMessageAsync("brag-nomatches", Settings.ChannelId.Commands, new FormatData(userId));
                return;
            }

            (var matchDataResult, var matchData) = await RiftBot.GetService<RiotService>()
                .GetMatchById(dbSummoner.SummonerRegion, matchlist.Random().GameId);

            if (matchDataResult != RequestResult.Success)
            {
                RiftBot.Log.Error("Failed to get match data");
                await RiftBot.SendMessageAsync(MessageService.Error, Settings.ChannelId.Commands);
                return ;
            }

            long participantId = matchData.ParticipantIdentities
                .First(x => x.Player.CurrentAccountId == dbSummoner.AccountId ||
                            x.Player.AccountId == dbSummoner.AccountId)
                .ParticipantId;

            var player = matchData.Participants.First(x => x.ParticipantId == participantId);

            if (player is null)
            {
                RiftBot.Log.Error("Failed to get player object");
                await RiftBot.SendMessageAsync(MessageService.Error, Settings.ChannelId.Commands);
                return ;
            }

            var champData = RiftBot.GetService<RiotService>().GetChampionById(player.ChampionId.ToString());

            if (champData is null)
            {
                RiftBot.Log.Error("Failed to obtain champ data");
                await RiftBot.SendMessageAsync(MessageService.Error, Settings.ChannelId.Commands);
                return ;
            }

            var champThumb = RiotService.GetChampionSquareByName(champData.Image);

            await DB.Cooldowns.SetLastBragTimeAsync(userId, DateTime.UtcNow);
            await DB.Statistics.AddAsync(userId, new StatisticData {BragsDone = 1u});

            var win = player.Stats.Win;
            var reward = new ItemReward().AddRandomCoins(
                win ? Settings.Economy.BragWinCoinsMin : Settings.Economy.BragLossCoinsMin,
                win ? Settings.Economy.BragWinCoinsMax : Settings.Economy.BragLossCoinsMax);

            await reward.DeliverToAsync(userId);
            OnUserBrag?.Invoke(null, new BragEventArgs(userId));

            var queue = RiftBot.GetService<RiotService>().GetQueueNameById(matchData.QueueId);

            var msg = await RiftBot.SendMessageAsync("brag-success", Settings.ChannelId.Chat, new FormatData(userId)
            {
                Brag = new BragData
                {
                    ChampionName = champData.Name,
                    ChampionPortraitUrl = champThumb,
                    Stats = player.Stats,
                    QueueName = queue,
                },
                Reward = reward
            });

            await RiftBot.SendMessageAsync(
                new IonicMessage(new RiftEmbed()
                    .WithTitle("Успешная хвастулька")
                    .WithDescription($"Дуй [сюда]({msg.GetJumpUrl()})"))
                , Settings.ChannelId.Commands);
        }

        static async Task<bool> CanBrag(ulong userId)
        {
            var data = await DB.Cooldowns.GetAsync(userId);

            if (data.BragTimeSpan == TimeSpan.Zero)
                return true;

            var diff = data.BragTimeSpan;

            return diff > Settings.Economy.BragCooldown;
        }
    }
}
