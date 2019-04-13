using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Embeds;
using Rift.Rewards;

using IonicLib;
using IonicLib.Extensions;
using IonicLib.Util;

namespace Rift.Services
{
    public class BotRespectService : RandomChanceReward
    {
        Timer timer;
        readonly Timer Starttimer;

        static readonly List<(uint, Reward)> AvailableRewards = new List<(uint, Reward)>
        {
            (13, new Reward(chests: 1)),
            (13, new Reward(customTickets: 2)),
            (13, new Reward(chests: 2)),
            (13, new Reward(giveawayTickets: 1)),
            (13, new Reward(coins: 1000)),
            (5, new Reward(powerupsDoubleExp: 1)),
            (1, new Reward(tokens: 1)),
            (10, new Reward(chests: 3)),
            (100, new Reward(coins: 500)),
        };

        public BotRespectService()
        {
            RiftBot.Log.Info($"Starting BotRespectService..");

            Starttimer = new Timer(async delegate { await InitTimer(); }, null, TimeSpan.FromSeconds(30),
                                   TimeSpan.Zero);
            foreach (var item in AvailableRewards)
            {
                item.Item2.CalculateReward();
                item.Item2.GenerateRewardString();
            }

            timer = new Timer(async delegate { await StartBotGifts(); }, null, Timeout.Infinite, 0);

            RiftBot.Log.Info($"BotRespectService loaded successfully.");
        }

        public Task InitTimer()
        {
            var nextGiftsTimeSpan = Helper.NextUInt(210, 330) * 60;
            timer.Change(TimeSpan.FromSeconds(nextGiftsTimeSpan), TimeSpan.Zero);

            RiftBot.Log.Debug($"Next gifts: {Helper.FromTimestamp(Helper.CurrentUnixTimestamp + nextGiftsTimeSpan).ToString()}");

            return Task.CompletedTask;
        }

        public async Task StartBotGifts()
        {
            RiftBot.Log.Debug("Gifts are on the way..");

            var users = await Database.GetBotRespectedUsersAsync();
            if (users.Length > 0)
            {
                foreach (var user in users)
                {
                    var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, user.UserId);
                    if (sgUser is null)
                        continue;

                    var reward = GetReward(AvailableRewards);
                    await reward.GiveRewardAsync(user.UserId);
                    await sgUser.SendEmbedAsync(BotRespectEmbeds.DMEmbed(reward));
                }


                if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
                    return;

                await chatChannel.SendEmbedAsync(BotRespectEmbeds.ChatEmbed);
            }
            else
                RiftBot.Log.Debug($"There was no users with bot respect");

            RiftBot.Log.Debug($"Finished sending gifts");

            await InitTimer();
        }
    }
}
