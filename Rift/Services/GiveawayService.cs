using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;
using Rift.Services.Message;

using Discord;
using IonicLib;
using IonicLib.Extensions;
using Rift.Services.Reward;

namespace Rift.Services
{
    public class GiveawayService
    {
        Timer eventTimer;
        
        public GiveawayService()
        {
            ScheduleTimer(TimeSpan.FromSeconds(10));
        }

        void ScheduleTimer(TimeSpan delay)
        {
            eventTimer = new Timer(async delegate { await CheckExpiredAsync(); }, null, delay, TimeSpan.Zero);
        }

        async Task CheckExpiredAsync()
        {
            var expiredEvents = await DB.ActiveGiveaways.GetExpiredAsync();

            if (expiredEvents is null || expiredEvents.Count == 0)
                return;

            foreach (var giveaway in expiredEvents)
            {
                await FinishGiveawayAsync(giveaway);
            }
        }

        async Task FinishGiveawayAsync(RiftGiveawayActive expiredGiveaway)
        {
            var dbGiveaway = await DB.Giveaways.GetAsync(expiredGiveaway.GiveawayName);

            var giveawayData = $"{expiredGiveaway.Id.ToString()}({expiredGiveaway.GiveawayName})";
            
            if (dbGiveaway is null)
            {
                RiftBot.Log.Error($"Could not finish giveaway \"{giveawayData}\": " +
                                  $"{nameof(RiftGiveaway)} is null!");
                return;
            }

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Comms, out var channel))
            {
                RiftBot.Log.Error($"Could not finish giveaway \"{giveawayData})\": " +
                                  "Giveaway channel is null!");
                return;
            }

            var message = (IUserMessage)await channel.GetMessageAsync(expiredGiveaway.ChannelMessageId);

            if (message is null)
            {
                RiftBot.Log.Error($"Could not finish giveaway \"{giveawayData})\": " +
                                  "Giveaway message is null! Deleted?");
                return;
            }

            if (!IonicClient.GetEmote(Settings.App.MainGuildId, "smite", out var emote))
            {
                RiftBot.Log.Error($"Could not finish giveaway \"{giveawayData})\": " +
                                  "Emote is null! Deleted?");
                return;
            }

            // Reaction amount is limited by discord itself.
            // See https://discordapp.com/developers/docs/resources/channel#get-reactions
            var reactions = await message.GetReactionUsersAsync(emote, 100).FlattenAsync();
            
            if (reactions is null)
            {
                RiftBot.Log.Error($"Could not finish giveaway \"{giveawayData})\": " +
                                  "Unable to get reactions.");
                return;
            }

            var reward = await DB.Rewards.GetAsync(dbGiveaway.RewardId);
            
            if (reward is null)
            {
                RiftBot.Log.Error($"Could not finish giveaway \"{giveawayData})\": " +
                                  $"Unable to get reward ID {dbGiveaway.RewardId.ToString()}.");
                return;
            }
            
            var participants = reactions
                .Where(x => !x.IsBot && x.Id != IonicClient.Client.CurrentUser.Id)
                .Select(x => x.Id)
                .ToArray();
            
            if (participants.Length == 0)
            {
                await LogGiveawayAsync(dbGiveaway, default, default,
                    reward.ToPlainString(), expiredGiveaway.StartedBy, expiredGiveaway.StartedAt);
                await DB.ActiveGiveaways.RemoveAsync(expiredGiveaway.Id);

                RiftBot.Log.Error($"Could not finish giveaway \"{giveawayData})\": " +
                                  "No participants.");
                return;
            }
            
            var winners = new ulong[dbGiveaway.WinnersAmount];

            if (participants.Length < dbGiveaway.WinnersAmount)
            {
                RiftBot.Log.Error($"Could not finish giveaway \"{giveawayData})\": " +
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
                    }
                    while (winners.Contains(winnerId));
                    
                    winners[i] = winnerId;
                }
            }

            foreach (var winner in winners)
            {
                await reward.DeliverToAsync(winner);
            }
            
            var log = new RiftGiveawayLog
            {
                Name = dbGiveaway.Name,
                Winners = winners,
                Participants = participants,
                Reward = reward.ToPlainString(),
                StartedBy = expiredGiveaway.StartedBy,
                StartedAt = expiredGiveaway.StartedAt,
                Duration = dbGiveaway.Duration,
                FinishedAt = DateTime.UtcNow,
            };

            await RiftBot.SendChatMessageAsync("giveaway-finished", new FormatData(expiredGiveaway.StartedBy)
            {
                Giveaway = new GiveawayData
                {
                    Log = log
                }
            });

            await LogGiveawayAsync(log).ConfigureAwait(false);
        }
        
        public async Task StartGiveawayAsync(string name, ulong callerId = 0u)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                RiftBot.Log.Warn("Empty giveaway name, skipping execution.");
                await RiftBot.SendMessageToDevelopers(new IonicMessage("Не указано название розыгрыша!"));
                return;
            }

            var giveaway = await DB.Giveaways.GetAsync(name);

            if (giveaway is null)
            {
                RiftBot.Log.Warn("Wrong giveaway name, skipping execution.");
                await RiftBot.SendMessageToDevelopers(new IonicMessage($"Розыгрыш с названием \"{name}\" отсутствует в моей базе данных."));
                return;
            }

            var msg = await DB.StoredMessages.GetMessageByIdAsync(giveaway.StoredMessageId);

            if (msg is null)
            {
                RiftBot.Log.Warn("Wrong giveaway message ID, skipping execution.");
                await RiftBot.SendMessageToDevelopers(new IonicMessage(
                    $"В настройках розыгрыша указан неверный ID сообщения: ({giveaway.StoredMessageId.ToString()})."));
                return;
            }

            var dbReward = await DB.Rewards.GetAsync(giveaway.RewardId);

            if (dbReward is null)
            {
                RiftBot.Log.Warn("Wrong giveaway reward ID, skipping execution.");
                await RiftBot.SendMessageToDevelopers(new IonicMessage(
                    $"В настройках розыгрыша указан неверный ID награды: ({giveaway.RewardId.ToString()})."));
                return;
            }

            var activeGiveaway = new RiftGiveawayActive
            {
                GiveawayName = giveaway.Name,
                StoredMessageId = giveaway.StoredMessageId,
                StartedBy = callerId == 0u ? IonicClient.Client.CurrentUser.Id : callerId,
                StartedAt = DateTime.UtcNow,
                DueTime = DateTime.UtcNow + giveaway.Duration
            };

            await DB.ActiveGiveaways.AddAsync(activeGiveaway).ConfigureAwait(false);

            var formattedMsg = await RiftBot.GetService<MessageService>().FormatMessageAsync(msg, new FormatData(callerId)
            {
                Giveaway = new GiveawayData
                {
                    Active = activeGiveaway,
                    Stored = giveaway
                },
                Reward = dbReward.ItemReward
            });

            await RiftBot.SendChatMessageAsync(formattedMsg).ConfigureAwait(false);
        }

        static async Task LogGiveawayAsync(RiftGiveawayLog log)
        {
            await DB.GiveawayLogs.AddAsync(log);
        }
        
        static async Task LogGiveawayAsync(RiftGiveaway giveaway, ulong[] winners, ulong[] participants, string rewardPlain, ulong startedBy, DateTime startedAt)
        {
            var log = new RiftGiveawayLog
            {
                Name = giveaway.Name,
                Winners = winners,
                Participants = participants,
                Reward = rewardPlain,
                StartedBy = startedBy,
                StartedAt = startedAt,
                Duration = giveaway.Duration,
                FinishedAt = DateTime.UtcNow,
            };

            await LogGiveawayAsync(log);
        }
    }
}
