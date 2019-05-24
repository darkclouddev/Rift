using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;
using Rift.Services.Message;
using Rift.Services.Message.Formatters;
using MessageType = Rift.Services.Message.MessageType;

using Humanizer;
using IonicLib;
using IonicLib.Util;
using Newtonsoft.Json;

namespace Rift.Services
{
    public class MessageService
    {
        public static readonly IonicMessage Error =
            new IonicMessage("$mention", new RiftEmbed()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription("Обратитесь к хранителю ботов и опишите ваши действия, которые привели к возникновению данной ошибки."));
        
        public static readonly IonicMessage UserNotFound =
            new IonicMessage("$mention", new RiftEmbed()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(255, 0, 0)
                .WithDescription("Пользователь не найден!"));
        
        public static readonly IonicMessage RoleNotFound =
            new IonicMessage("$mention", new RiftEmbed()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(255, 0, 0)
                .WithDescription("Роль не найдена!"));
        
        public MessageService()
        {
            RiftBot.Log.Info($"Starting up {nameof(MessageService)}.");

            var sw = new Stopwatch();
            sw.Restart();

            formatters = new ConcurrentDictionary<string, FormatterBase>();

            foreach (var type in GetFormatters())
            {
                var formatter = (FormatterBase)Activator.CreateInstance(type);
                formatters.TryAdd(formatter.Template, formatter);
            }

            sw.Stop();
            RiftBot.Log.Info($"Loaded {formatters.Count.ToString()} message formatters in" +
                $" {sw.Elapsed.Humanize(1, new CultureInfo("en-US")).ToLowerInvariant()}.");

            RiftBot.Log.Info($"Starting up message scheduler.");
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

        static ConcurrentDictionary<string, FormatterBase> formatters;

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

        public async Task<IonicMessage> GetMessageAsync(string identifier, FormatData data)
        {
            var mapping = await Database.GetMessageMappingByNameAsync(identifier);

            if (mapping is null)
            {
                RiftBot.Log.Warn($"Message mapping \"{identifier}\" does not exist.");
                return Error;
            }

            var dbMessage = await Database.GetMessageByIdAsync(mapping.MessageId);
            
            if (dbMessage is null)
            {
                RiftBot.Log.Warn($"Message with ID \"{mapping.MessageId.ToString()}\" does not exist.");
                return Error;
            }

            return await FormatMessageAsync(dbMessage, data);
        }

        const string TemplateRegex = @"\$\w+";

        public async Task<IonicMessage> FormatMessageAsync(RiftMessage message, FormatData data = null)
        {
            if (formatters.Count == 0)
                return new IonicMessage(message);

            if (message is null)
            {
                RiftBot.Log.Error("Message is empty!");
                return Error;
            }

            if (string.IsNullOrWhiteSpace(message.Text)
                && string.IsNullOrWhiteSpace(message.Embed)
                && string.IsNullOrWhiteSpace(message.ImageUrl))
            {
                RiftBot.Log.Error($"Message \"{message.Id.ToString()}\" has no data!");
                return Error;
            }

            if (message.ApplyFormat)
            {
                var matches = new List<Match>();

                if (!string.IsNullOrWhiteSpace(message.Text))
                    matches.AddRange(Regex.Matches(message.Text, TemplateRegex));

                if (!string.IsNullOrWhiteSpace(message.Embed))
                    matches.AddRange(Regex.Matches(message.Embed, TemplateRegex));

                foreach (var match in matches)
                {
                    if (!formatters.TryGetValue(match.Value, out var f))
                        continue;

                    try
                    {
                        await f.Format(message, data);
                    }
                    catch (Exception ex)
                    {
                        RiftBot.Log.Error(ex);
                    }
                }
            }
            
            return new IonicMessage(message);
        }

        public async Task PutEmbedToDatabase(string name, RiftEmbed embed)
        {
            string json = null;

            try
            {
                json = JsonConvert.SerializeObject(embed, Formatting.Indented,
                    new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});
            }
            catch (Exception ex)
            {
                RiftBot.Log.Error(ex);
            }

            var message = new RiftMessage
            {
                ApplyFormat = true,
                Name = name,
                Embed = json,
            };

            await Database.AddStoredMessage(message);
        }

        #endregion Message formatting
    }
}
