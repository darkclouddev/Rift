using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Services.Message;

using IonicLib;
using IonicLib.Util;

namespace Rift.Services
{
    public class MessageService
    {
        static ConcurrentDictionary<Guid, SendMessageBase> toSend = new ConcurrentDictionary<Guid, SendMessageBase>();

        static ConcurrentDictionary<Guid, DeleteMessageBase> toDelete =
            new ConcurrentDictionary<Guid, DeleteMessageBase>();

        static Timer checkTimer;

        public MessageService()
        {
            checkTimer = new Timer(async delegate { await CheckMessagesAsync(); }, null, TimeSpan.FromSeconds(10),
                                   TimeSpan.FromSeconds(5));
        }

        public bool TryAddSend(SendMessageBase message) => toSend.TryAdd(message.Id, message);
        public bool TryAddDelete(DeleteMessageBase message) => toDelete.TryAdd(message.Id, message);

        async Task CheckMessagesAsync()
        {
            var dtNow = DateTime.Now;

            await CheckSendAsync(dtNow);
            await CheckDeleteAsync(dtNow);
        }

        async Task CheckSendAsync(DateTime dt)
        {
            var unsentId = toSend.Values.ToList()
                                 .Where(x => x.DeliveryTime < dt)
                                 .Take(3)
                                 .OrderBy(x => x.AddedOn)
                                 .Select(x => x.Id);

            if (unsentId is null || unsentId.Count() == 0)
                return;

            foreach (var id in unsentId)
            {
                (bool success, var message) = TryRemoveSend(id);

                if (!success)
                    continue;

                await DeliverAsync(message);
            }
        }

        async Task CheckDeleteAsync(DateTime dt)
        {
            var undeletedId = toDelete.Values.ToList().Where(x => x.DeletionTime < dt).Take(3).Select(x => x.Id);

            if (undeletedId is null || undeletedId.Count() == 0)
                return;

            foreach (var id in undeletedId)
            {
                (bool success, var message) = TryRemoveDelete(id);

                if (!success)
                    continue;

                await DeleteAsync(message);
            }
        }

        async Task DeliverAsync(SendMessageBase message)
        {
            switch (message.DestinationType)
            {
                case DestinationType.DM:

                    var user = IonicClient.GetGuildUserById(Settings.App.MainGuildId, message.DestinationId);

                    if (user is null)
                        return;

                    var userChannel = await user.GetOrCreateDMChannelAsync();

                    switch (message.MessageType)
                    {
                        case MessageType.PlainText:

                            await userChannel.SendMessageAsync(message.Text);
                            break;

                        case MessageType.Embed:

                            await userChannel.SendEmbedAsync(message.Embed);
                            break;

                        case MessageType.Mixed:

                            await userChannel.SendMessageAsync(message.Text, embed: message.Embed);
                            break;
                    }

                    break;

                case DestinationType.GuildChannel:

                    if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, message.DestinationId, out var channel))
                        return;

                    switch (message.MessageType)
                    {
                        case MessageType.PlainText:

                            await channel.SendMessageAsync(message.Text);
                            break;

                        case MessageType.Embed:

                            await channel.SendEmbedAsync(message.Embed);
                            break;

                        case MessageType.Mixed:

                            await channel.SendMessageAsync(message.Text, embed: message.Embed);
                            break;
                    }

                    break;
            }
        }

        async Task DeleteAsync(DeleteMessageBase message)
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, message.ChannelId, out var channel))
                return;

            try
            {
                var msg = await channel.GetMessageAsync(message.MessageId);

                if (msg is null)
                    return;

                await msg?.DeleteAsync();
            }
            catch
            {
            }
        }

        static (bool, SendMessageBase) TryRemoveSend(Guid id)
        {
            bool result = toSend.TryRemove(id, out var message);

            return (result, message);
        }

        static (bool, DeleteMessageBase) TryRemoveDelete(Guid id)
        {
            bool result = toDelete.TryRemove(id, out var message);

            return (result, message);
        }

        public int GetSendQueueLength() => toSend.Count;
        public int GetDeleteQueueLength() => toDelete.Count;
    }
}
