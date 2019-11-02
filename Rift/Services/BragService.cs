using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Discord;

using IonicLib.Extensions;

using Settings = Rift.Configuration.Settings;

using Rift.Database;
using Rift.Events;
using Rift.Services.Message;
using Rift.Services.Reward;
using Rift.Services.Interfaces;

namespace Rift.Services
{
    public class BragService : IBragService
    {
        public event EventHandler<BragEventArgs> OnUserBrag;

        static readonly SemaphoreSlim Mutex = new SemaphoreSlim(1);

        readonly IMessageService messageService;
        readonly IRiotService riotService;
        readonly IRewardService rewardService;

        public BragService(IMessageService messageService,
                           IRiotService riotService,
                           IRewardService rewardService)
        {
            this.messageService = messageService;
            this.riotService = riotService;
            this.rewardService = rewardService;
        }
        
        public async Task GetUserBragAsync(ulong userId)
        {
            await Mutex.WaitAsync().ConfigureAwait(false);

            try
            {
                await GetUserBragInternalAsync(userId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                RiftBot.Log.Error(ex, "Failed to brag:");
            }
            finally
            {
                Mutex.Release();
            }
        }

        async Task GetUserBragInternalAsync(ulong userId)
        {
            if (!await CanBrag(userId))
            {
                await messageService.SendMessageAsync("brag-cooldown", Settings.ChannelId.Commands, new FormatData(userId));
                return;
            }

            var dbSummoner = await DB.LeagueData.GetAsync(userId);
            if (dbSummoner is null || string.IsNullOrWhiteSpace(dbSummoner.AccountId))
            {
                await messageService.SendMessageAsync("loldata-nodata", Settings.ChannelId.Commands, new FormatData(userId));
                return;
            }

            (var matchlistResult, var matchlist) =
                await riotService.GetLast20MatchesByAccountIdAsync(dbSummoner.SummonerRegion, dbSummoner.AccountId);
            if (matchlistResult != RequestResult.Success)
            {
                await messageService.SendMessageAsync("brag-nomatches", Settings.ChannelId.Commands, new FormatData(userId));
                return;
            }

            (var matchDataResult, var matchData) =
                await riotService.GetMatchById(dbSummoner.SummonerRegion, matchlist.Random().GameId);
            if (matchDataResult != RequestResult.Success)
            {
                RiftBot.Log.Error("Failed to get match data");
                await messageService.SendMessageAsync(MessageService.Error, Settings.ChannelId.Commands);
                return;
            }

            long participantId = matchData.ParticipantIdentities
                .First(x => x.Player.CurrentAccountId == dbSummoner.AccountId ||
                            x.Player.AccountId == dbSummoner.AccountId)
                .ParticipantId;

            var player = matchData.Participants.First(x => x.ParticipantId == participantId);
            if (player is null)
            {
                RiftBot.Log.Error("Failed to get player object");
                await messageService.SendMessageAsync(MessageService.Error, Settings.ChannelId.Commands);
                return;
            }

            var champData = riotService.GetChampionById(player.ChampionId.ToString());
            if (champData is null)
            {
                RiftBot.Log.Error("Failed to obtain champ data");
                await messageService.SendMessageAsync(MessageService.Error, Settings.ChannelId.Commands);
                return;
            }

            var champThumb = riotService.GetChampionSquareByName(champData.Image);

            await DB.Cooldowns.SetLastBragTimeAsync(userId, DateTime.UtcNow);
            await DB.Statistics.AddAsync(userId, new StatisticData {BragsDone = 1u});

            var win = player.Stats.Win;
            var reward = new ItemReward().AddRandomCoins(
                win ? Settings.Economy.BragWinCoinsMin : Settings.Economy.BragLossCoinsMin,
                win ? Settings.Economy.BragWinCoinsMax : Settings.Economy.BragLossCoinsMax);

            await rewardService.DeliverToAsync(userId, reward);
            OnUserBrag?.Invoke(null, new BragEventArgs(userId));

            var queue = riotService.GetQueueNameById(matchData.QueueId);

            var msg = await messageService.SendMessageAsync("brag-success", Settings.ChannelId.Chat, new FormatData(userId)
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
            
            await messageService.SendMessageAsync("brag-success-link", Settings.ChannelId.Commands, new FormatData(userId)
            {
                MessageData = new MessageData
                {
                    Link = msg.GetJumpUrl()
                }
            });
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
