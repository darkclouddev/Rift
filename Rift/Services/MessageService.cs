using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;
using Rift.Services.Message;
using Rift.Services.Message.Formatters;

using IonicLib;
using IonicLib.Util;

namespace Rift.Services
{
    public class MessageService
    {
        public MessageService()
        {
            formatters = GetFormatters();
            
            checkTimer = new Timer(
                async delegate
                {
                    await CheckMessagesAsync();
                },
                null,
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(1));
        }
        
        //TODO: refactor using scheduling timer
        #region Delayed messages
        
        static ConcurrentDictionary<Guid, SendMessageBase> toSend = new ConcurrentDictionary<Guid, SendMessageBase>();

        static ConcurrentDictionary<Guid, DeleteMessageBase> toDelete =
            new ConcurrentDictionary<Guid, DeleteMessageBase>();

        static Timer checkTimer;

        public bool TryAddSend(SendMessageBase message) => toSend.TryAdd(message.Id, message);
        public bool TryAddDelete(DeleteMessageBase message) => toDelete.TryAdd(message.Id, message);

        async Task CheckMessagesAsync()
        {
            var dtNow = DateTime.UtcNow;

            await CheckSendAsync(dtNow);
            await CheckDeleteAsync(dtNow);
        }

        async Task CheckSendAsync(DateTime dt)
        {
            var unsentId = toSend.Values.ToList()
                .Where(x => x.DeliveryTime < dt)
                .OrderBy(x => x.AddedOn)
                .Take(3)
                .Select(x => x.Id)
                .ToList();

            if (!unsentId.Any())
                return;

            foreach (var id in unsentId)
            {
                (var success, var message) = TryRemoveSend(id);

                if (!success)
                    continue;

                await DeliverAsync(message);
            }
        }

        async Task CheckDeleteAsync(DateTime dt)
        {
            var undeletedIds = toDelete.Values.ToList()
                .Where(x => x.DeletionTime < dt)
                .Take(3)
                .Select(x => x.Id)
                .ToList();

            if (!undeletedIds.Any())
                return;

            foreach (var id in undeletedIds)
            {
                (var success, var message) = TryRemoveDelete(id);

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

                await msg.DeleteAsync();
            }
            catch(Exception ex) // fails when message is already deleted, no delete perms or discord outage
            {
                RiftBot.Log.Error(ex);
            }
        }

        static (bool, SendMessageBase) TryRemoveSend(Guid id)
        {
            var result = toSend.TryRemove(id, out var message);

            return (result, message);
        }

        static (bool, DeleteMessageBase) TryRemoveDelete(Guid id)
        {
            var result = toDelete.TryRemove(id, out var message);

            return (result, message);
        }

        public int GetSendQueueLength() => toSend.Count;
        public int GetDeleteQueueLength() => toDelete.Count;
        
        #endregion Delayed messages
        
        #region Message formatting

        static List<Type> formatters;

        static List<Type> GetFormatters()
        {
            var assembly = Assembly.GetEntryAssembly();

            if (assembly is null)
            {
                RiftBot.Log.Error("Unable to obtain entry assembly, disabling formatting service.");
                return null;
            }
            
            return assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(FormatterBase))).ToList();
        }

        public async Task<(bool, IonicMessage)> GetMessageAsync(string identifier, ulong userId)
        {
            var mapping = await Database.GetMessageMappingByNameAsync(identifier);

            if (mapping is null)
            {
                RiftBot.Log.Warn($"Message mapping \"{identifier}\" does not exist.");
                return (false, null);
            }

            var dbMessage = await Database.GetMessageByIdAsync(mapping.MessageId);
            
            if (dbMessage is null)
            {
                RiftBot.Log.Warn($"Message with ID \"{mapping.MessageId.ToString()}\" does not exist.");
                return (false, null);
            }

            return (true, FormatMessage(dbMessage, userId));
        }
        
        public IonicMessage FormatMessage(RiftMessage message, ulong userId)
        {
            if (message is null || userId == 0ul)
                return null;

            if (string.IsNullOrWhiteSpace(message.Text)
                && string.IsNullOrWhiteSpace(message.Embed)
                && string.IsNullOrWhiteSpace(message.ImageUrl))
                return null;

            if (message.ApplyFormat)
            {
                try
                {
                    foreach (var type in formatters)
                    {
                        var formatter = (FormatterBase) Activator.CreateInstance(type);
                        message = formatter.Format(message, userId);
                    }
                }
                catch (Exception ex)
                {
                    RiftBot.Log.Warn(ex);
                    throw;
                }
            }
            
            return new IonicMessage(message);
        }

        #endregion Message formatting
    }
}
