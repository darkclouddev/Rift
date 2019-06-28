using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Services.Reward;
using Rift.Util;

using Humanizer;
using IonicLib;
using IonicLib.Extensions;

namespace Rift.Services
{
    public class BotRespectService
    {
        Timer timer;
        readonly Timer Starttimer;

        static readonly List<(uint, ItemReward)> AvailableRewards = new List<(uint, ItemReward)>
        {
            (13, new ItemReward().AddChests(1)),
            (13, new ItemReward().AddTickets(2)),
            (13, new ItemReward().AddChests(2)),
            (13, new ItemReward().AddCoins(1000)),
            (5, new ItemReward().AddDoubleExps(1)),
            (1, new ItemReward().AddTokens(1)),
            (10, new ItemReward().AddChests(3)),
            (100, new ItemReward().AddCoins(500)),
        };

        public BotRespectService()
        {
            RiftBot.Log.Info("Starting BotRespectService..");

            Starttimer = new Timer(
                async delegate
                {
                    await InitTimer();
                },
                null,
                TimeSpan.FromSeconds(30),
                TimeSpan.Zero);

            timer = new Timer(
                async delegate
                {
                    await StartBotGifts();
                },
                null,
                Timeout.Infinite,
                0);

            RiftBot.Log.Info("BotRespectService loaded successfully.");
        }

        public Task InitTimer()
        {
            var nextGiftsTimeSpan = Helper.NextUInt(210, 330) * 60;
            timer.Change(TimeSpan.FromSeconds(nextGiftsTimeSpan), TimeSpan.Zero);

            RiftBot.Log.Debug($"Next gifts: {Helper.FromTimestamp(Helper.CurrentUnixTimestamp + nextGiftsTimeSpan).Humanize()}");

            return Task.CompletedTask;
        }

        public async Task StartBotGifts()
        {
            RiftBot.Log.Debug("Gifts are on the way..");

            var users = await DB.Cooldowns.GetBotRespectedUsersAsync();
            if (users.Count > 0)
            {
                foreach ((var userId, var dt) in users)
                {
                    var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);
                    if (sgUser is null)
                        continue;

                    (var chance, var reward) = AvailableRewards.Random();
                    await reward.DeliverToAsync(userId);
                }

                if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Comms, out var commsChannel))
                    return;

                var rewardMsg = await RiftBot.GetMessageAsync("botrespect-success", null);
                await commsChannel.SendIonicMessageAsync(rewardMsg);
            }
            else
                RiftBot.Log.Debug("There was no users with bot respect");

            RiftBot.Log.Debug("Finished sending gifts");

            await InitTimer();
        }
    }
}
