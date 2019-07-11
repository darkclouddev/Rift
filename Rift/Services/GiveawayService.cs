using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Settings = Rift.Configuration.Settings;

using Rift.Data.Models;
using Rift.Database;
using Rift.Events;
using Rift.Services.Message;

using Discord;

using Humanizer;

using IonicLib;
using IonicLib.Extensions;

namespace Rift.Services
{
    public class GiveawayService
    {
        public static event EventHandler<GiveawaysParticipatedEventArgs> GiveawaysParticipated;

        Timer eventTimer;
        readonly CultureInfo schedulerCulture;

        public GiveawayService()
        {
            schedulerCulture = new CultureInfo("en-US");
            InitializeTimer(TimeSpan.FromSeconds(5));
        }

        void InitializeTimer(TimeSpan delay)
        {
            eventTimer = new Timer(async delegate { await CheckExpiredAsync(); }, null, delay, TimeSpan.Zero);
        }

        async Task ScheduleTimerToClosest()
        {
            var closest = await DB.ActiveGiveaways.GetClosestAsync();

            if (closest is null)
                return;

            if (DateTime.UtcNow > closest.DueTime)
                return;

            var ts = closest.DueTime - DateTime.UtcNow;

            RiftBot.Log.Info($"Giveaway tracker scheduled to {closest.DueTime.Humanize(culture: schedulerCulture)}.");

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

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Comms, out var channel))
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

            if (!IonicClient.GetEmote(Settings.App.MainGuildId, "smite", out var emote))
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
                               .Where(x => !x.IsBot && x.Id != IonicClient.Client.CurrentUser.Id)
                               .Select(x => x.Id)
                               .ToArray();

            if (participants.Length == 0)
            {
                await LogGiveawayAsync(dbGiveaway.Name, null, null,
                                       dbReward.ToPlainString(), expiredGiveaway.StartedBy, expiredGiveaway.StartedAt,
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

            foreach (var winner in winners) await reward.DeliverToAsync(winner);

            await DB.ActiveGiveaways.RemoveAsync(expiredGiveaway.Id);

            foreach (var participant in participants)
                GiveawaysParticipated?.Invoke(null, new GiveawaysParticipatedEventArgs(participant));

            var log = new RiftGiveawayLog
            {
                Name = dbGiveaway.Name,
                Winners = winners,
                Participants = participants,
                Reward = dbReward.ToPlainString(),
                StartedBy = expiredGiveaway.StartedBy,
                StartedAt = expiredGiveaway.StartedAt,
                Duration = dbGiveaway.Duration,
                FinishedAt = DateTime.UtcNow,
            };

            await RiftBot.SendChatMessageAsync("giveaway-finished", new FormatData(expiredGiveaway.StartedBy)
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
                RiftBot.Log.Warn("Empty giveaway name, skipping execution.");
                return;
            }

            var giveaway = await DB.Giveaways.GetAsync(name);

            if (giveaway is null)
            {
                RiftBot.Log.Warn("Wrong giveaway name, skipping execution.");
                return;
            }

            var msg = await DB.StoredMessages.GetMessageByIdAsync(giveaway.StoredMessageId);

            if (msg is null)
            {
                RiftBot.Log.Warn("Wrong giveaway message ID, skipping execution.");
                return;
            }

            var dbReward = await DB.Rewards.GetAsync(giveaway.RewardId);

            if (dbReward is null)
            {
                RiftBot.Log.Warn("Wrong giveaway reward ID, skipping execution.");
                return;
            }

            var reward = dbReward.ToRewardBase();

            if (!IonicClient.GetEmote(Settings.App.MainGuildId, "smite", out var smite))
            {
                RiftBot.Log.Warn("No giveaway emote, skipping execution.");
                return;
            }

            var activeGiveaway = new RiftActiveGiveaway
            {
                GiveawayName = giveaway.Name,
                StoredMessageId = giveaway.StoredMessageId,
                StartedBy = startedById == 0u ? IonicClient.Client.CurrentUser.Id : startedById,
                StartedAt = DateTime.UtcNow,
                DueTime = DateTime.UtcNow + giveaway.Duration,
            };

            var formattedMsg = await RiftBot.GetService<MessageService>().FormatMessageAsync(
                msg, new FormatData(startedById)
                {
                    Giveaway = new GiveawayData
                    {
                        Stored = giveaway
                    },
                    Reward = reward
                });

            var giveawayMessage = await RiftBot.SendChatMessageAsync(formattedMsg).ConfigureAwait(false);

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
                RiftBot.Log.Warn("Wrong reward ID, skipping execution.");
                return;
            }

            var reward = dbReward.ToRewardBase();

            var usersWithTickets = await DB.Inventory.GetAsync(x => x.Tickets > 0);

            if (usersWithTickets is null || usersWithTickets.Count == 0)
            {
                RiftBot.Log.Warn("No users with tickets, skipping execution.");
                return;
            }

            RiftBot.Log.Info(
                $"Ticket giveaway started for {usersWithTickets.Count.ToString()}, reward ID {rewardId.ToString()}.");

            var removeInv = new InventoryData {Tickets = 1u};

            foreach (var userInventory in usersWithTickets)
            {
                //await DB.Inventory.RemoveAsync(userInventory.UserId, removeInv); // too slow on tunnel db connection
            }

            var winnerId = usersWithTickets.Random().UserId;

            await reward.DeliverToAsync(winnerId);

            await RiftBot.SendChatMessageAsync("giveaway-ticket-success", new FormatData(startedBy)
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
                Reward = dbReward.ToPlainString(),
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

            RiftBot.Log.Info($"Gived tickets to {affectedRows.ToString()} users");

            await RiftBot.SendChatMessageAsync("ticket-charity-success", new FormatData(startedBy));
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
