using System;
using System.Threading.Tasks;

using Rift.Data.Models;
using Rift.Services.Message;

using IonicLib;

namespace Rift.Services
{
    public class GiveawayService
    {
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
                RiftBot.Log.Warn("Wrong giveaway name, skipping execution.");
                await RiftBot.SendMessageToDevelopers(new IonicMessage(
                    $"В настройках розыгрыша указан неверный ID сообщения: ({giveaway.StoredMessageId})."));
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
                    ActiveGiveaway = activeGiveaway,
                    StoredGiveaway = giveaway
                }
            });

            await RiftBot.SendChatMessageAsync(formattedMsg).ConfigureAwait(false);
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

            await DB.GiveawayLogs.AddAsync(log);
        }
    }
}
