using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Settings = Rift.Configuration.Settings;

using Rift.Database;
using Rift.Events;
using Rift.Services.Economy;
using Rift.Services.Message;
using Rift.Services.Reward;

using IonicLib.Extensions;

namespace Rift.Services
{
    public class BragService
    {
        public static EventHandler<BragEventArgs> OnUserBrag;

        static readonly SemaphoreSlim mutex = new SemaphoreSlim(1);

        public async Task<IonicMessage> GetUserBragAsync(ulong userId)
        {
            await mutex.WaitAsync().ConfigureAwait(false);

            IonicMessage result;

            try
            {
                result = await GetUserBragInternalAsync(userId).ConfigureAwait(false);
            }
            finally
            {
                mutex.Release();
            }

            return result;
        }

        static async Task<IonicMessage> GetUserBragInternalAsync(ulong userId)
        {
            if (!await CanBrag(userId))
                return await RiftBot.GetMessageAsync("brag-cooldown", new FormatData(userId));

            var dbSummoner = await DB.LeagueData.GetAsync(userId);

            if (dbSummoner is null || string.IsNullOrWhiteSpace(dbSummoner.AccountId))
                return await RiftBot.GetMessageAsync("loldata-nodata", new FormatData(userId));

            (var matchlistResult, var matchlist) = await RiftBot.GetService<RiotService>()
                                                                .GetLast20MatchesByAccountIdAsync(
                                                                    dbSummoner.SummonerRegion, dbSummoner.AccountId);

            if (matchlistResult != RequestResult.Success)
                return await RiftBot.GetMessageAsync("brag-nomatches", new FormatData(userId));

            (var matchDataResult, var matchData) = await RiftBot.GetService<RiotService>()
                                                                .GetMatchById(
                                                                    dbSummoner.SummonerRegion,
                                                                    matchlist.Random().GameId);

            if (matchDataResult != RequestResult.Success)
            {
                RiftBot.Log.Error("Failed to get match data");
                return MessageService.Error;
            }

            long participantId = matchData.ParticipantIdentities
                                          .First(x => x.Player.CurrentAccountId == dbSummoner.AccountId ||
                                                      x.Player.AccountId == dbSummoner.AccountId)
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

            await DB.Cooldowns.SetLastBragTimeAsync(userId, DateTime.UtcNow);
            await DB.Statistics.AddAsync(userId, new StatisticData {BragsDone = 1u});

            var brag = new Brag(player.Stats.Win);
            await DB.Inventory.AddAsync(userId, new InventoryData {Coins = brag.Coins});
            OnUserBrag?.Invoke(null, new BragEventArgs(userId));

            var queue = RiftBot.GetService<RiotService>().GetQueueNameById(matchData.QueueId);

            return await RiftBot.GetMessageAsync("brag-success", new FormatData(userId)
            {
                Brag = new BragData
                {
                    ChampionName = champData.Name,
                    ChampionPortraitUrl = champThumb,
                    Stats = player.Stats,
                    QueueName = queue,
                },
                Reward = new ItemReward().AddCoins(brag.Coins)
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
