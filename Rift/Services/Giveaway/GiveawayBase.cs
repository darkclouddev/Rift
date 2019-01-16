using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Embeds;
using Rift.Rewards;
using Rift.Services.Message;

using IonicLib;
using IonicLib.Extensions;
using IonicLib.Util;

namespace Rift.Services.Giveaway
{
    public abstract class GiveawayBase
    {
        protected const uint duration = 5;
        public Reward reward = null;
        protected Timer timer;
        protected ulong winner;
        protected List<ulong> users;

        public GiveawayBase(Reward reward)
        {
            this.reward = reward;
        }

        public virtual Task StartGiveawayAsync()
        {
            return Task.CompletedTask;
        }

        public async Task FinishGiveaway()
        {
            if (users.Count > 0)
            {
                winner = users.Random();
                RiftBot.Log.Debug($"Giveaway winner: {winner}");
                await reward.GiveRewardAsync(winner);
            }
            else
            {
                RiftBot.Log.Debug($"No winner");
            }

            await Terminate();
        }

        public virtual async Task Terminate()
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Giveaways, out var channel))
                return;
            await channel.SendEmbedAsync(GiveawayEmbeds.ChatWinner(winner, reward));

            RiftBot.GetService<MessageService>()
                .TryAddSend(new EmbedMessage(DestinationType.DM, winner, TimeSpan.Zero,
                                             GiveawayEmbeds.DMWinner(reward)));

            Clean();
        }

        public void Clean()
        {
            reward = null;
            winner = 0;
            users = null;
            timer = null;
        }
    }

    public enum TicketType
    {
        NoTicket,
        Usual,
        Rare,
    }
}
