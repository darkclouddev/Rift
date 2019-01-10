using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Discord;
using Discord.WebSocket;

using Rift.Configuration;
using Rift.Embeds;
using Rift.Rewards;

using IonicLib;
using IonicLib.Util;

namespace Rift.Services.Giveaway
{
    public class FreeGiveaway : GiveawayBase
    {
        GuildEmote giveawayEmote;
        ulong giveawayMessageId;

        public FreeGiveaway(Reward reward)
            : base(reward)
        {
        }

        Task Client_AddReactedUser(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel,
                                   SocketReaction reaction)
        {
            if (message.Id != giveawayMessageId)
                return Task.CompletedTask;

            if (giveawayEmote is null)
                return Task.CompletedTask;

            if (users == null || users.Contains(reaction.UserId) || reaction.UserId == 404611580819537920)
                return Task.CompletedTask;

            if (!reaction.Emote.Name.Equals(giveawayEmote.Name, StringComparison.InvariantCultureIgnoreCase))
                return Task.CompletedTask;

            users.Add(reaction.UserId);

            return Task.CompletedTask;
        }

        public override async Task StartGiveawayAsync()
        {
            if (giveawayEmote is null)
            {
                if (!IonicClient.GetEmote(Settings.App.EmoteGuildId, "giveaway", out var emote))
                {
                    Clean();
                    RiftBot.Log.Error("Failed to get giveaway emote");
                    return;
                }

                giveawayEmote = emote;
            }

            try
            {
                users = new List<ulong>();
                if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Giveaways,
                                                out var channel))
                    return;

                await channel.SendMessageAsync(GiveawayEmbeds.FreeMessage);
                var msg = await channel.SendEmbedAsync(GiveawayEmbeds.ChatFreeEmbed(reward));
                giveawayMessageId = msg.Id;

                IonicClient.Client.ReactionAdded += Client_AddReactedUser;

                await msg.AddReactionAsync(giveawayEmote);

                timer = new Timer(async delegate { await FinishGiveaway(); }, null, TimeSpan.FromMinutes(duration),
                                  TimeSpan.Zero);
            }
            catch (Exception ex)
            {
                RiftBot.Log.Error(ex, "GiveawayService exception");
            }
        }

        public override async Task Terminate()
        {
            IonicClient.Client.ReactionAdded -= Client_AddReactedUser;
            await base.Terminate();
            giveawayMessageId = 0;
        }
    }
}
