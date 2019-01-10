using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data;
using Rift.Data.Models;
using Rift.Embeds;
using Rift.Rewards;
using Rift.Services;

using IonicLib;
using IonicLib.Extensions;
using IonicLib.Util;

using Discord;
using Discord.WebSocket;

namespace Rift.Services
{
    public class EventService
    {
        static readonly TimeSpan eventDuration = TimeSpan.FromMinutes(5);

        Timer startupTimer;

        Timer eventTimer;
        Timer eventEndTimer;
        GuildEmote eventEmote;
        Reward reward;
        Reward winnerReward;
        EventType eventType;

        public EventService()
        {
            RiftBot.Log.Info($"Starting EventService..");

            startupTimer = new Timer(async delegate { await SetupNextEvent(); }, null, TimeSpan.FromSeconds(15),
                                     TimeSpan.Zero);

            RiftBot.Log.Info($"EventService loaded successfully.");
        }

        static async Task<ScheduledEvent> GetNextEvent(DateTime dt)
        {
            return (await RiftBot.GetService<DatabaseService>()
                                 .GetEventsAsync(x =>
                                                     x.DayId == (int) dt.DayOfWeek
                                                     && dt < dt.Date + new TimeSpan(x.Hour, x.Minute, 0)))
                .FirstOrDefault();
        }

        async Task SetupNextEvent()
        {
            if (!(await RiftBot.GetService<DatabaseService>().GetAllEventsAsync()).Any())
            {
                RiftBot.Log.Warn($"No events in db, skipping event setup.");
                return;
            }

            var dt = DateTime.Now;
            var day = dt.DayOfWeek;

            ScheduledEvent eventData;

            do
            {
                eventData = await GetNextEvent(dt);

                if (eventData is null)
                    dt = dt.Date.AddDays(1);
            }
            while (eventData is null);

            eventType = (EventType) eventData.EventId;
            switch (eventType)
            {
                case EventType.Baron:
                    reward = EventReward.BaronGeneral;
                    winnerReward = EventReward.BaronWinner;
                    winnerReward.CalculateReward();
                    winnerReward.GenerateRewardString();
                    break;
                case EventType.Drake:
                    reward = EventReward.DrakeGeneral;
                    winnerReward = EventReward.DrakeWinner;
                    winnerReward.CalculateReward();
                    winnerReward.GenerateRewardString();
                    break;
                case EventType.BlueBuff:
                case EventType.Krug:
                case EventType.Razorfins:
                case EventType.RedBuff:
                case EventType.Wolves:
                    reward = EventReward.RandomReward;
                    break;
            }

            reward.CalculateReward();
            reward.GenerateRewardString();

            var eventTs = GetEventTimeSpan(dt, eventData.Hour, eventData.Minute);

            RiftBot.Log.Debug($"Event Type: {eventType.ToString()}");
            RiftBot.Log.Debug($"Event Time: {(DateTime.Now + eventTs).ToString(RiftBot.Culture)}");

            eventTimer = new Timer(async delegate { await StartEvent(eventType); }, null, eventTs, TimeSpan.Zero);
        }

        static TimeSpan GetEventTimeSpan(DateTime date, int hour, int minute)
        {
            var eventTime = date.Date + new TimeSpan(hour, minute, 0);

            return eventTime - DateTime.Now;
        }

        static List<ulong> reactionIds = new List<ulong>();
        static ulong eventMessageId = 0ul;

        Task Client_AddReactedUser(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel,
                                   SocketReaction reaction)
        {
            if (message.Id != eventMessageId)
                return Task.CompletedTask;

            if (eventEmote is null)
                return Task.CompletedTask;

            if (!reaction.Emote.Name.Equals(eventEmote.Name, StringComparison.InvariantCultureIgnoreCase))
                return Task.CompletedTask;

            if (reactionIds.Contains(reaction.UserId))
                return Task.CompletedTask;

            reactionIds.Add(reaction.UserId);

            return Task.CompletedTask;
        }

        public async Task StartEvent(EventType eventType)
        {
            if (eventEmote is null)
            {
                if (!IonicClient.GetEmote(Settings.App.MainGuildId, "smite", out var emote))
                    return;

                eventEmote = emote;
            }

            await EnableChat(false);

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var channel))
                return;

            Embed eventEmbed = null;

            switch (eventType)
            {
                case EventType.Baron:
                    eventEmbed = EventEmbeds.BaronEmbed(reward);
                    break;

                case EventType.Drake:
                    eventEmbed = EventEmbeds.DrakeEmbed(reward);
                    break;

                case EventType.Wolves:
                    eventEmbed = EventEmbeds.WolvesEmbed(reward);
                    break;

                case EventType.Razorfins:
                    eventEmbed = EventEmbeds.RazorfinsEmbed(reward);
                    break;

                case EventType.Krug:
                    eventEmbed = EventEmbeds.KrugEmbed(reward);
                    break;

                case EventType.RedBuff:
                    eventEmbed = EventEmbeds.RedBuffEmbed(reward);
                    break;

                case EventType.BlueBuff:
                    eventEmbed = EventEmbeds.BlueBuffEmbed(reward);
                    break;

                default:
                    RiftBot.Log.Error($"Wrong event type: {eventType.ToString()}");
                    return;
            }

            await channel.SendEmbedAsync(EventEmbeds.embedMsg);

            IonicClient.Client.ReactionAdded += Client_AddReactedUser;

            var msg = await channel.SendMessageAsync($"Призыватели, @here, атакуйте и получайте награды.",
                                                     embed: eventEmbed);

            await msg.AddReactionAsync(eventEmote);

            eventMessageId = msg.Id;

            eventEndTimer = new Timer(async delegate { await EndEvent(eventType); },
                                      null,
                                      eventDuration,
                                      TimeSpan.Zero);
        }

        async Task EndEvent(EventType eventType)
        {
            IonicClient.Client.ReactionAdded -= Client_AddReactedUser;

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var channel))
                return;

            RiftBot.Log.Debug($"EndEvent Reactions: {reactionIds.Count}");

            if (reactionIds.Count() != 0)
            {
                foreach (var id in reactionIds)
                {
                    await reward.GiveRewardAsync(id);
                }

                switch (eventType)
                {
                    case EventType.Baron:
                    case EventType.Drake:
                    {
                        ulong winnerId = reactionIds.Random();
                        await winnerReward.GiveRewardAsync(winnerId);

                        await channel.SendEmbedAsync(EventEmbeds.WinnerEmbed(winnerId, winnerReward.RewardString));
                        break;
                    }
                }
            }

            await channel.SendEmbedAsync(EventEmbeds.UserCountEmbed(reactionIds.Count()));

            reactionIds = new List<ulong>();
            eventMessageId = 0ul;

            await EnableChat(true);

            await SetupNextEvent();
        }

        async Task EnableChat(bool enable)
        {
            if (!IonicClient.GetGuild(Settings.App.MainGuildId, out var guild))
                return;

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var channel))
                return;

            await DoEnableChatForRole(guild.EveryoneRole)
                .ContinueWith(async x =>
                {
                    if (IonicClient.GetRole(Settings.App.MainGuildId, Settings.RoleId.Moderator, out var modRole))
                        await DoEnableChatForRole(modRole);
                });

            async Task DoEnableChatForRole(IRole role)
            {
                var perms = channel.GetPermissionOverwrite(role);

                if (perms.HasValue)
                {
                    var newPerms = perms.Value.Modify(sendMessages: enable ? PermValue.Inherit : PermValue.Deny);
                    await channel.AddPermissionOverwriteAsync(role, newPerms);
                }
                else
                {
                    await channel.AddPermissionOverwriteAsync(role,
                                                              new OverwritePermissions(sendMessages: enable
                                                                                           ? PermValue.Inherit
                                                                                           : PermValue.Deny));
                }
            }
        }
    }

    public enum EventType
    {
        Baron = 0,
        Drake = 1,
        Wolves = 2,
        Razorfins = 3,
        Krug = 4,
        RedBuff = 5,
        BlueBuff = 6,
    }
}
