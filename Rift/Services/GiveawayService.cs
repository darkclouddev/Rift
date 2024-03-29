﻿using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Settings = Rift.Configuration.Settings;

using Rift.Data.Models;
using Rift.Database;
using Rift.Events;
using Rift.Services.Interfaces;
using Rift.Services.Message;

using Discord;

using Humanizer;

using IonicLib;
using IonicLib.Extensions;

namespace Rift.Services
{
    public class GiveawayService : IGiveawayService
    {
        public event EventHandler<GiveawaysParticipatedEventArgs> GiveawaysParticipated;

        Timer eventTimer;
        readonly CultureInfo schedulerCulture;
        readonly IMessageService messageService;
        readonly IRewardService rewardService;

        public GiveawayService(IMessageService messageService,
                               IRewardService rewardService)
        {
            this.messageService = messageService;
            this.rewardService = rewardService;
            
            schedulerCulture = new CultureInfo("en-US");
            InitializeTimer(TimeSpan.FromSeconds(5));
        }

        void InitializeTimer(TimeSpan delay)
        {
            eventTimer = new Timer(
                async delegate { await CheckExpiredAsync(); },
                null,
                delay,
                TimeSpan.Zero);
        }

        async Task ScheduleTimerToClosest()
        {
            var closest = await DB.ActiveGiveaways.GetClosestAsync();

            if (closest is null)
                return;

            if (DateTime.UtcNow > closest.DueTime)
                return;

            var ts = closest.DueTime - DateTime.UtcNow;

            RiftBot.Log.Information($"Giveaway tracker scheduled to {closest.DueTime.Humanize(culture: schedulerCulture)}.");

            eventTimer.Change(ts, TimeSpan.Zero);
        }

        async Task CheckExpiredAsync()
        {
            var expiredEvents = await DB.ActiveGiveaways.GetExpiredAsync();

            if (expiredEvents is null || expiredEvents.Count == 0)
            {
                await ScheduleTimerToClosest().ConfigureAwait(false);
                return;
            }

            foreach (var giveaway in expiredEvents) await FinishGiveawayAsync(giveaway);

            var all = await DB.ActiveGiveaways.GetAllAsync();

            if (all is null)
                return;

            await ScheduleTimerToClosest().ConfigureAwait(false);
        }

        async Task FinishGiveawayAsync(RiftActiveGiveaway expiredGiveaway)
        {
            var dbGiveaway = await DB.Giveaways.GetAsync(expiredGiveaway.GiveawayName);

            var giveawayData = $"ID {expiredGiveaway.Id.ToString()} \"{expiredGiveaway.GiveawayName}\"";

            if (dbGiveaway is null)
            {
                RiftBot.Log.Error($"Could not finish giveaway {giveawayData}: {nameof(RiftGiveaway)} is null!");
                return;
            }

            if (!IonicHelper.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var channel))
            {
                RiftBot.Log.Error($"Could not finish giveaway {giveawayData}: Giveaway channel is null!");
                return;
            }

            var message = (IUserMessage) await channel.GetMessageAsync(expiredGiveaway.ChannelMessageId);

            if (message is null)
            {
                RiftBot.Log.Error($"Could not finish giveaway {giveawayData}: Giveaway message is null! Deleted?");
                return;
            }

            if (!IonicHelper.GetEmote(403616665603932162, "giveaway", out var emote))
            {
                RiftBot.Log.Error($"Could not finish giveaway {giveawayData}: Emote is null! Deleted?");
                return;
            }

            // Reaction amount is limited by discord itself.
            // See https://discordapp.com/developers/docs/resources/channel#get-reactions
            var reactions = await message.GetReactionUsersAsync(emote, 100).FlattenAsync();

            if (reactions is null)
            {
                RiftBot.Log.Error($"Could not finish giveaway {giveawayData}: Unable to get reactions.");
                return;
            }

            var dbReward = await DB.Rewards.GetAsync(dbGiveaway.RewardId);

            if (dbReward is null)
            {
                RiftBot.Log.Error($"Could not finish giveaway {giveawayData}: " +
                                  $"Unable to get reward ID {dbGiveaway.RewardId.ToString()}.");
                return;
            }

            var reward = dbReward.ToRewardBase();

            var participants = reactions
                               .Where(x => !x.IsBot && x.Id != IonicHelper.Client.CurrentUser.Id)
                               .Select(x => x.Id)
                               .ToArray();

            if (participants.Length == 0)
            {
                await LogGiveawayAsync(dbGiveaway.Name, null, null,
                                       "No reward provided", expiredGiveaway.StartedBy, expiredGiveaway.StartedAt,
                                       dbGiveaway.Duration);
                await DB.ActiveGiveaways.RemoveAsync(expiredGiveaway.Id);

                RiftBot.Log.Error($"Could not finish giveaway {giveawayData}: No participants.");
                return;
            }

            var winners = new ulong[dbGiveaway.WinnersAmount];

            if (participants.Length < dbGiveaway.WinnersAmount)
            {
                RiftBot.Log.Error($"Could not finish giveaway {giveawayData}: " +
                                  $"Not enough participants: only {participants.Length.ToString()} of minimum {dbGiveaway.WinnersAmount.ToString()}");
                return;
            }

            if (participants.Length == dbGiveaway.WinnersAmount)
            {
                Array.Copy(participants, winners, dbGiveaway.WinnersAmount);
            }
            else
            {
                ulong winnerId;

                for (var i = 0; i < dbGiveaway.WinnersAmount; i++)
                {
                    do
                    {
                        winnerId = participants.Random();
                    } while (winners.Contains(winnerId));

                    winners[i] = winnerId;
                }
            }

            foreach (var winner in winners)
                await rewardService.DeliverToAsync(winner, reward);

            await DB.ActiveGiveaways.RemoveAsync(expiredGiveaway.Id);

            foreach (var participant in participants)
                GiveawaysParticipated?.Invoke(null, new GiveawaysParticipatedEventArgs(participant));

            var log = new RiftGiveawayLog
            {
                Name = dbGiveaway.Name,
                Winners = winners,
                Participants = participants,
                Reward = "No reward provided",
                StartedBy = expiredGiveaway.StartedBy,
                StartedAt = expiredGiveaway.StartedAt,
                Duration = dbGiveaway.Duration,
                FinishedAt = DateTime.UtcNow,
            };

            await messageService.SendMessageAsync("giveaway-finished", Settings.ChannelId.Chat,
                new FormatData(expiredGiveaway.StartedBy)
                {
                    Giveaway = new GiveawayData
                    {
                        Log = log,
                        Stored = dbGiveaway,
                    },
                    Reward = reward          
                });

            await LogGiveawayAsync(log).ConfigureAwait(false);
        }

        public async Task StartGiveawayAsync(string name, ulong startedById)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                RiftBot.Log.Warning("Empty giveaway name, skipping execution.");
                return;
            }

            var giveaway = await DB.Giveaways.GetAsync(name);

            if (giveaway is null)
            {
                RiftBot.Log.Warning("Wrong giveaway name, skipping execution.");
                return;
            }

            var msg = await messageService.GetMessageFromApi(giveaway.MessageId);

            if (msg is null)
            {
                RiftBot.Log.Warning("Wrong giveaway message ID, skipping execution.");
                return;
            }

            var dbReward = await DB.Rewards.GetAsync(giveaway.RewardId);

            if (dbReward is null)
            {
                RiftBot.Log.Warning("Wrong giveaway reward ID, skipping execution.");
                return;
            }

            var reward = dbReward.ToRewardBase();

            if (!IonicHelper.GetEmote(403616665603932162, "giveaway", out var smite))
            {
                RiftBot.Log.Warning("No giveaway emote, skipping execution.");
                return;
            }

            var activeGiveaway = new RiftActiveGiveaway
            {
                GiveawayName = giveaway.Name,
                MessageId = giveaway.MessageId,
                StartedBy = startedById == 0u ? IonicHelper.Client.CurrentUser.Id : startedById,
                StartedAt = DateTime.UtcNow,
                DueTime = DateTime.UtcNow + giveaway.Duration,
            };

            var formattedMsg = await messageService.FormatMessageAsync(
                msg, new FormatData(startedById)
                {
                    Giveaway = new GiveawayData
                    {
                        Stored = giveaway
                    },
                    Reward = reward
                });

            var giveawayMessage = await messageService.SendMessageAsync(formattedMsg, Settings.ChannelId.Chat).ConfigureAwait(false);

            activeGiveaway.ChannelMessageId = giveawayMessage.Id;

            await DB.ActiveGiveaways.AddAsync(activeGiveaway).ConfigureAwait(false);

            await giveawayMessage.AddReactionAsync(smite);

            await ScheduleTimerToClosest().ConfigureAwait(false);
        }

        public async Task StartTicketGiveawayAsync(int rewardId, ulong startedBy)
        {
            var dbReward = await DB.Rewards.GetAsync(rewardId);

            if (dbReward is null)
            {
                RiftBot.Log.Warning("Wrong reward ID, skipping execution.");
                return;
            }

            var reward = dbReward.ToRewardBase();

            var usersWithTickets = await DB.Inventory.GetAsync(x => x.Tickets >= 1);

            if (usersWithTickets is null || usersWithTickets.Count == 0)
            {
                RiftBot.Log.Warning("No users with tickets, skipping execution.");
                return;
            }

            RiftBot.Log.Information($"Ticket giveaway started for {usersWithTickets.Count.ToString()}, reward ID {rewardId.ToString()}.");

            var removeInv = new InventoryData {Tickets = 1u};

            var userList = usersWithTickets.ToList();
            var winnerId = 0ul;
            
            do
            {
                if (userList.Count == 0)
                {
                    RiftBot.Log.Warning("No users on server participated, skipping execution.");
                    return;
                } 
                
                var winner = userList.Random();
                winnerId = winner.UserId;
                userList.Remove(winner);
            } while (!IonicHelper.GetGuildUserById(Settings.App.MainGuildId, winnerId, out var sgUser));

            foreach (var userInventory in usersWithTickets)
            {
                await DB.Inventory.RemoveAsync(userInventory.UserId, removeInv); // very slow on tunneled db connection
            }
            
            await rewardService.DeliverToAsync(winnerId, reward);

            await messageService.SendMessageAsync("giveaway-ticket-success", Settings.ChannelId.Chat, new FormatData(startedBy)
            {
                Giveaway = new GiveawayData
                {
                    TicketGiveaway = new TicketGiveaway
                    {
                        ParticipantsCount = usersWithTickets.Count,
                        WinnerId = winnerId
                    }
                },
                Reward = reward
            });

            var log = new RiftGiveawayLog
            {
                Name = "Ticket Giveaway",
                Winners = new[] {winnerId},
                Participants = usersWithTickets.Select(x => x.UserId).ToArray(),
                Reward = "No reward provided",
                StartedBy = startedBy,
                StartedAt = DateTime.Now,
                Duration = TimeSpan.Zero,
                FinishedAt = DateTime.UtcNow,
            };

            await DB.GiveawayLogs.AddAsync(log);
        }

        public async Task GiveTicketsToLowLevelUsersAsync(ulong startedBy)
        {
            var affectedRows = await DB.Inventory.GiveTicketsToUsersAsync();

            RiftBot.Log.Information($"Gived tickets to {affectedRows.ToString()} users");

            await messageService.SendMessageAsync("ticket-charity-success", Settings.ChannelId.Chat, new FormatData(startedBy));
        }

        static async Task LogGiveawayAsync(RiftGiveawayLog log)
        {
            await DB.GiveawayLogs.AddAsync(log);
        }

        static async Task LogGiveawayAsync(string name, ulong[] winners, ulong[] participants, string rewardPlain,
                                           ulong startedBy, DateTime startedAt, TimeSpan duration)
        {
            var log = new RiftGiveawayLog
            {
                Name = name,
                Winners = winners,
                Participants = participants,
                Reward = rewardPlain,
                StartedBy = startedBy,
                StartedAt = startedAt,
                Duration = duration,
                FinishedAt = DateTime.UtcNow,
            };

            await LogGiveawayAsync(log);
        }
    }
}
