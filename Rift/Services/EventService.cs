using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;
using Rift.Services.Message;
using Rift.Services.Reward;
using Rift.Util;

using Discord;
using Discord.WebSocket;
using IonicLib;

namespace Rift.Services
{
    public class EventService
    {
        static readonly TimeSpan eventDuration = TimeSpan.FromMinutes(5);

        Timer startupTimer;

        Timer eventTimer;
        Timer eventEndTimer;
        GuildEmote eventEmote;
        ItemReward reward;
        ItemReward rewardWinner;
        EventType eventType;

        public EventService()
        {
            RiftBot.Log.Info($"Starting EventService..");

            startupTimer = new Timer(
                async delegate
                {
                    await SetupNextEvent();
                },
                null,
                TimeSpan.FromSeconds(15),
                TimeSpan.Zero);

            RiftBot.Log.Info($"EventService loaded successfully.");
        }

        static async Task<ScheduledEvent> GetNextEvent(DateTime dt)
        {
            return (await Database.GetEventsAsync(x =>
                    x.DayId == (int) dt.DayOfWeek && dt < dt.Date + new TimeSpan(x.Hour, x.Minute, 0)))
                .FirstOrDefault();
        }

        async Task SetupNextEvent()
        {
            if (!(await Database.GetAllEventsAsync()).Any())
            {
                RiftBot.Log.Warn($"No events in db, skipping event setup.");
                return;
            }

            var dt = DateTime.Now;

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
                case EventType.BlueBuff:
                case EventType.RedBuff:
                    reward = new ItemReward().AddCoins(1_000u).AddTokens(1u).AddDoubleExps(1u);
                    break;

                case EventType.Razorfins:
                case EventType.ScuttleCrab:
                case EventType.Krugs:
                    reward = new ItemReward().AddCoins(1_000u).AddChests(1u);
                    break;

                case EventType.Wolves:
                case EventType.Gromp:
                    reward = new ItemReward().AddCoins(1_000u).AddTokens(1u);
                    break;

                case EventType.Drake:
                case EventType.Baron:
                    reward = new ItemReward().AddCoins(13_000u).AddTokens(6u).AddChests(3u);
                    rewardWinner = new ItemReward().AddCoins(30_000u).AddTokens(6u).AddTickets(1u);
                    break;

                case EventType.Armadillo:
                case EventType.BigEyes:
                case EventType.DevastatorCrab:
                case EventType.CrookedTail:
                    reward = new ItemReward().AddTokens(8u).AddSpheres(1u);
                    rewardWinner = new ItemReward().AddCoins(50_000u).AddTokens(10u).AddTickets(1u);
                    break;
            }

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

        Task Client_AddReactedUser(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
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

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Comms, out var channel))
                return;

            IonicMessage eventMessage = null;

            switch (eventType)
            {
                case EventType.Baron:
                    eventMessage = await RiftBot.GetMessageAsync("event-start-baron", null);
                    break;

                case EventType.Drake:
                    eventMessage = await RiftBot.GetMessageAsync("event-start-drake", null);
                    break;

                case EventType.Wolves:
                    eventMessage = await RiftBot.GetMessageAsync("event-start-wolves", null);
                    break;

                case EventType.Razorfins:
                    eventMessage = await RiftBot.GetMessageAsync("event-start-razorfins", null);
                    break;

                case EventType.Krugs:
                    eventMessage = await RiftBot.GetMessageAsync("event-start-krugs", null);
                    break;

                case EventType.RedBuff:
                    eventMessage = await RiftBot.GetMessageAsync("event-start-redbuff", null);
                    break;

                case EventType.BlueBuff:
                    eventMessage = await RiftBot.GetMessageAsync("event-start-bluebuff", null);
                    break;

                default:
                    RiftBot.Log.Error($"Wrong event type: {eventType.ToString()}");
                    return;
            }

            eventMessage = new IonicMessage("Призыватели, @here, атакуйте и получайте награды.", eventMessage.Embed, eventMessage.ImageUrl);

            var msgEventStart = await RiftBot.GetMessageAsync("event-start", null);
            await channel.SendIonicMessageAsync(msgEventStart);

            IonicClient.Client.ReactionAdded += Client_AddReactedUser;

            var msg = await channel.SendIonicMessageAsync(eventMessage);
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

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Comms, out var channel))
                return;

            RiftBot.Log.Debug($"EndEvent Reactions: {reactionIds.Count.ToString()}");

            if (reactionIds.Count != 0)
            {
                foreach (var id in reactionIds)
                {
                    await reward.DeliverToAsync(id);
                }

                //switch (eventType)
                //{
                //    case EventType.Baron:
                //    case EventType.Drake:
                //    {
                //        ulong winnerId = reactionIds.Random();
                //        await winnerReward.GiveRewardAsync(winnerId);

                //        var msgEventWinner = await RiftBot.GetMessageAsync("event-winner", null);
                //        await channel.SendIonicMessageAsync(msgEventWinner);
                //        break;
                //    }
                //}
            }

            var msgEventParticipants = await RiftBot.GetMessageAsync("event-end-participants", null);
            await channel.SendIonicMessageAsync(msgEventParticipants);

            reactionIds = new List<ulong>();
            eventMessageId = 0ul;

            await EnableChat(true);

            await SetupNextEvent();
        }

        async Task EnableChat(bool enable)
        {
            if (!IonicClient.GetGuild(Settings.App.MainGuildId, out var guild))
                return;

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Comms, out var channel))
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
                        new OverwritePermissions(sendMessages: enable ? PermValue.Inherit : PermValue.Deny));
                }
            }
        }
    }

    public enum EventType
    {
        Wolves = 0,
        Razorfins = 1,
        Krugs = 2,
        Gromp = 3,
        ScuttleCrab = 4,
        BlueBuff = 5,
        RedBuff = 6,
        Drake = 7,
        Baron = 8,
        Armadillo = 9,
        BigEyes = 10,
        DevastatorCrab = 11,
        CrookedTail = 12,
    }
}
