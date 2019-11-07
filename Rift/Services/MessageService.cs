using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Discord;

using Rift.Configuration;
using Rift.Data.Models;
using Rift.Services.Interfaces;
using Rift.Services.Message;
using Rift.Services.Message.Templates;
using Rift.Util;

using MessageType = Rift.Services.Message.MessageType;

using Humanizer;

using IonicLib;
using IonicLib.Util;

using Newtonsoft.Json;

namespace Rift.Services
{
    public class MessageService : IMessageService
    {
        public static readonly IonicMessage Error =
            new IonicMessage(new RiftEmbed()
                             .WithAuthor("Ошибка")
                             .WithColor(226, 87, 76)
                             .WithDescription(
                                 "Обратитесь к хранителю ботов и опишите ваши действия, которые привели к возникновению данной ошибки.")
                             .WithThumbnailUrl("https://cdn.ionpri.me/rift/error.jpg"));

        public static readonly IonicMessage UserNotFound =
            new IonicMessage(new RiftEmbed()
                             .WithAuthor("Ошибка")
                             .WithColor(255, 0, 0)
                             .WithDescription("Пользователь не найден!"));

        public static readonly IonicMessage RoleNotFound =
            new IonicMessage(new RiftEmbed()
                             .WithAuthor("Ошибка")
                             .WithColor(255, 0, 0)
                             .WithDescription("Роль не найдена!"));

        readonly HttpClient client;
        
        readonly IEmoteService emoteService;
        
        public MessageService(IEmoteService emoteService)
        {
            this.emoteService = emoteService;
            
            RiftBot.Log.Information($"Starting up {nameof(MessageService)}.");

            client = new HttpClient();
            
            var sw = new Stopwatch();
            sw.Restart();

            templates = new ConcurrentDictionary<string, ITemplate>();

            foreach (var type in GetTemplates())
            {
                var templateObj = (TemplateBase) Activator.CreateInstance(type);
                templates.TryAdd(templateObj.Template, templateObj);
            }

            sw.Stop();
            RiftBot.Log.Information($"Loaded {templates.Count.ToString()} message templates in" +
                             $" {sw.Elapsed.Humanize(1, new CultureInfo("en-US")).ToLowerInvariant()}.");

            RiftBot.Log.Information("Starting up message scheduler.");
            checkTimer = new Timer(
                async delegate { await CheckMessagesAsync(); },
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

        public bool TryAddSend(SendMessageBase message)
        {
            return toSend.TryAdd(message.Id, message);
        }

        public bool TryAddDelete(DeleteMessageBase message)
        {
            return toDelete.TryAdd(message.Id, message);
        }

        async Task CheckMessagesAsync()
        {
            var dtNow = DateTime.UtcNow;

            await CheckSendAsync(dtNow).ConfigureAwait(false);
            await CheckDeleteAsync(dtNow).ConfigureAwait(false);
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

                await DeliverAsync(message).ConfigureAwait(false);
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

                await DeleteAsync(message).ConfigureAwait(false);
            }
        }

        async Task DeliverAsync(SendMessageBase message)
        {
            switch (message.DestinationType)
            {
                case DestinationType.DM:

                    if (!IonicHelper.GetGuildUserById(Settings.App.MainGuildId, message.DestinationId, out var sgUser))
                        return;

                    var userChannel = await sgUser.GetOrCreateDMChannelAsync();

                    switch (message.MessageType)
                    {
                        case MessageType.PlainText:

                            await userChannel.SendMessageAsync(message.Text).ConfigureAwait(false);
                            break;

                        case MessageType.Embed:

                            await userChannel.SendEmbedAsync(message.Embed).ConfigureAwait(false);
                            break;

                        case MessageType.Mixed:

                            await userChannel.SendMessageAsync(message.Text, embed: message.Embed)
                                             .ConfigureAwait(false);
                            break;
                    }

                    break;

                case DestinationType.GuildChannel:

                    if (!IonicHelper.GetTextChannel(Settings.App.MainGuildId, message.DestinationId, out var channel))
                        return;

                    switch (message.MessageType)
                    {
                        case MessageType.PlainText:

                            await channel.SendMessageAsync(message.Text).ConfigureAwait(false);
                            break;

                        case MessageType.Embed:

                            await channel.SendEmbedAsync(message.Embed).ConfigureAwait(false);
                            break;

                        case MessageType.Mixed:

                            await channel.SendMessageAsync(message.Text, embed: message.Embed).ConfigureAwait(false);
                            break;
                    }

                    break;
            }
        }

        async Task DeleteAsync(DeleteMessageBase message)
        {
            if (!IonicHelper.GetTextChannel(Settings.App.MainGuildId, message.ChannelId, out var channel))
                return;

            try
            {
                var msg = await channel.GetMessageAsync(message.MessageId);

                if (msg is null)
                    return;

                await msg.DeleteAsync().ConfigureAwait(false);
            }
            catch (Exception ex) // fails when message is already deleted, no delete perms or discord outage
            {
                RiftBot.Log.Error(ex, "Message was already deleted or no permissions?");
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

        public int GetSendQueueLength()
        {
            return toSend.Count;
        }

        public int GetDeleteQueueLength()
        {
            return toDelete.Count;
        }

        #endregion Delayed messages
        
        #region Message formatting

        static ConcurrentDictionary<string, ITemplate> templates;

        static List<Type> GetTemplates()
        {
            var assembly = Assembly.GetEntryAssembly();

            if (assembly is null)
            {
                RiftBot.Log.Error("Unable to obtain entry assembly, disabling template service.");
                return null;
            }

            return assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(TemplateBase))).ToList();
        }

        public List<ITemplate> GetActiveTemplates()
        {
            return templates.Values.ToList();
        }

        public async Task<RiftMessage> GetMessageFromApi(Guid id)
        {
            var requestUrl = $"http://localhost:7727/v1/messages/{id.ToString()}";
            var response = await client.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
                return null;

            try
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<RiftMessage>(json);
            }
            catch (Exception ex) 
            {
                RiftBot.Log.Error(ex, "failed to read API response!");
                return null;
            }
        }
        
        public async Task<IonicMessage> GetMessageAsync(string identifier, FormatData data)
        {
            var mapping = await DB.Mappings.GetByNameAsync(identifier);
            if (mapping is null)
            {
                RiftBot.Log.Warning($"Message mapping \"{identifier}\" does not exist.");
                return Error;
            }
            
            var dbMessage = await GetMessageFromApi(mapping.MessageId);
            if (dbMessage is null)
            {
                RiftBot.Log.Warning($"Message with ID \"{mapping.MessageId.ToString()}\" does not exist.");
                return Error;
            }

            return await FormatMessageAsync(dbMessage, data);
        }

        const string TemplateRegex = @"\$\w+";
        const string EmotePrefix = "$emote";

        public async Task<IonicMessage> FormatMessageAsync(RiftMessage message, FormatData data = null)
        {
            if (templates.Count == 0)
                return new IonicMessage(message);

            if (message is null)
            {
                RiftBot.Log.Error("Message is empty!");
                return Error;
            }

            if (string.IsNullOrWhiteSpace(message.Text)
                && string.IsNullOrWhiteSpace(message.EmbedJson)
                && string.IsNullOrWhiteSpace(message.ImageUrl))
            {
                RiftBot.Log.Error($"Message \"{message.Id.ToString()}\" has no data!");
                return Error;
            }
            
            var matches = new List<Match>();

            if (!string.IsNullOrWhiteSpace(message.Text))
                matches.AddRange(Regex.Matches(message.Text, TemplateRegex));

            if (!string.IsNullOrWhiteSpace(message.EmbedJson))
                matches.AddRange(Regex.Matches(message.EmbedJson, TemplateRegex));

            foreach (var match in matches)
            {
                if (!templates.TryGetValue(match.Value, out var template))
                {
                    if (!match.Value.StartsWith(EmotePrefix))
                        continue;

                    emoteService.FormatMessage(match.Value, message);
                    continue;
                }

                try
                {
                    await template.ApplyAsync(message, data).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    RiftBot.Log.Error(ex, "An error occured while applying template");
                }
            }

            return new IonicMessage(message);
        }
        
        public async Task<IUserMessage> SendMessageAsync(string identifier, ulong channelId, FormatData data)
        {
            if (!IonicHelper.GetTextChannel(Settings.App.MainGuildId, channelId, out var channel))
                return null;

            var msg = await GetMessageAsync(identifier, data);

            if (msg is null)
                return null;

            return await channel.SendIonicMessageAsync(msg).ConfigureAwait(false);
        }
        
        public async Task<IUserMessage> SendMessageAsync(IonicMessage message, ulong channelId)
        {
            if (!IonicHelper.GetTextChannel(Settings.App.MainGuildId, channelId, out var channel))
                return null;

            return await channel.SendIonicMessageAsync(message);
        }

        #endregion Message formatting
    }
}
