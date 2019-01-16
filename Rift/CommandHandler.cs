﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;

using Rift.Configuration;
using Rift.Services;
using Rift.Services.Message;

using IonicLib;
using IonicLib.Util;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Microsoft.Extensions.DependencyInjection;

namespace Rift
{
    public class CommandHandler
    {
        const char commandPrefix = '!';

        public readonly IServiceProvider provider;
        static DiscordSocketClient client;
        static CommandService commandService;
        static DatabaseService databaseService;
        static EconomyService economyService;
        static RoleService roleService;
        static RiotService riotService;
        static MinionService minionService;
        static MessageService messageService;
        static GiveawayService giveawayService;

        public CommandHandler(IServiceProvider serviceProvider)
        {
            provider = serviceProvider;

            client = provider.GetService<DiscordSocketClient>();
            commandService = provider.GetService<CommandService>();
            databaseService = provider.GetService<DatabaseService>();
            economyService = provider.GetService<EconomyService>();
            minionService = provider.GetService<MinionService>();
            roleService = provider.GetService<RoleService>();
            messageService = provider.GetService<MessageService>();
            riotService = provider.GetService<RiotService>();
            giveawayService = provider.GetService<GiveawayService>();
        }

        public async Task ConfigureAsync()
        {
            await commandService.AddModulesAsync(Assembly.GetEntryAssembly(), provider);
            economyService.Init();

            await riotService.InitAsync();

            client.MessageReceived += ProcessMessage;
            client.UserJoined += UserJoined;
            client.UserLeft += UserLeft;
            client.Ready += Ready;
        }

        public static async Task Ready()
        {
            await client.SetGameAsync(RiftBot.BotStatusMessage);

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var channel))
                return;

            await channel.SendEmbedAsync(new EmbedBuilder()
                                         .WithDescription("Основной бот сервера включен.")
                                         .Build());
        }

        async Task ProcessMessage(SocketMessage socketMsg)
        {
            if (!(socketMsg is SocketUserMessage message))
                return;

            if (Settings.App.MaintenanceMode
                && !RiftBot.IsAdmin(message.Author)
                && !RiftBot.IsModerator(message.Author))
                return;

            var context = new CommandContext(client, message);

            if (await HandleCommand(context))
                return;

            await HandlePlainText(context);
        }

        async Task<bool> HandleCommand(CommandContext context)
        {
            int argPos = 0;

            if (context.IsPrivate)
            {
                var result = await commandService.ExecuteAsync(context, argPos, provider);

                if (result.IsSuccess)
                {
                    RiftBot.Log.Info($"{context.Message.Author} sent DM command: \"{context.Message.Content}\"");
                }
                else
                {
                    if (result.Error != null && result.Error.Value != CommandError.UnknownCommand)
                        RiftBot.Log
                               .Error($"{context.Message.Author} | {context.Message.Content} | {result.ErrorReason}");

                    await context.User.SendMessageAsync(result.ErrorReason);
                }

                return true;
            }
            
            if (context.Message.HasCharPrefix(commandPrefix, ref argPos))
            {
                var result = await commandService.ExecuteAsync(context, argPos, provider);

                if (result.IsSuccess)
                {
                    string command = context.Message.Content.Substring(1).TrimStart();

                    RiftBot.Log.Info($"{context.Message.Author} sent channel command: \"{command}\"");
                }
                else
                {
                    if (result.Error != null && result.Error.Value != CommandError.UnknownCommand)
                        RiftBot.Log.Error(result.ErrorReason);

                    await context.User.SendMessageAsync(result.ErrorReason);
                }

                messageService.TryAddDelete(new DeleteMessage(context.Message, TimeSpan.FromSeconds(5)));
                return true;
            }

            return false;
        }

        async Task HandlePlainText(CommandContext context)
        {
            if (context.Message.Channel.Id != Settings.ChannelId.Chat)
                return;

            if (Settings.Chat.AttachmentFilterEnabled && !RiftBot.IsAdmin(context.Message.Author))
            {
                if (context.Message.Attachments != null && context.Message.Attachments.Count > 0)
                    await context.Message.DeleteAsync();
            }

            if (Settings.Chat.CapsFilterEnabled && await HasCaps(context.Message.Content))
            {
                await context.Message.DeleteAsync();

                var eb = new EmbedBuilder()
                    .WithDescription($":no_entry_sign: {context.Message.Author.Mention} капсить на сервере запрещено!");

                await context.User.SendEmbedAsync(eb);

                return;
            }

            if (Settings.Chat.UrlFilterEnabled && HasURL(context.Message.Content))
            {
                await context.Message.DeleteAsync();

                var eb = new EmbedBuilder()
                    .WithDescription($"{Settings.Emote.ExMark} {context.Message.Author.Mention} ссылки на сервере запрещены!");

                await context.User.SendEmbedAsync(eb);

                return;
            }

            if (IsEligibleForEconomy(context.User.Id))
                await economyService.ProcessMessageAsync(context.Message);
        }

        async Task UserJoined(SocketGuildUser user)
        {
            if (user.Guild.Id != Settings.App.MainGuildId)
                return;

            await RegisterJoinedLeft(user, UserState.Joined);

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
                return;

            var eb = new EmbedBuilder()
                .WithDescription($"Призыватель **{user.Username}** подключился к серверу.");

            await chatChannel.SendEmbedAsync(eb);
        }

        async Task RegisterJoinedLeft(SocketGuildUser sgUser, UserState state)
        {
            if (state == UserState.Joined)
            {
                await roleService.RestoreTempRolesAsync(sgUser);

                if (sgUser.Guild.Id == Settings.App.MainGuildId)
                {
                    //if (welcomeEmbed == null)
                    //{
                    //	var eb2 = new EmbedBuilder()
                    //		.WithImageUrl("https://cdn.discordapp.com/attachments/404205665893089280/404734705934532608/00386e05e1aa5211.png")
                    //		.WithTitle($"Добро пожаловать на {sgUser.Guild.Name}")
                    //		.AddField($"Боевая система сервера", $"Общение <#{Settings.ChannelId.Chat}> - поднимайте уровень и получайте награды.\n" +
                    //					$"Узнайте всё о боевой системе более подробно: <#{Settings.ChannelId.Information}>\n\n" +
                    //					$"{Settings.Emote.QuestionMark} Команда в чат: `!помощь`. (все команды бота)");

                    //	welcomeEmbed = eb2.Build();
                    //}

                    //await sgUser.SendEmbedAsync(welcomeEmbed);
                }
            }

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Logs, out var usersChannel))
                return;

            var eb = new EmbedBuilder()
                     .WithColor(state == UserState.Joined ? new Color(46, 204, 113) : new Color(231, 76, 60))
                     .WithAuthor(new EmbedAuthorBuilder
                     {
                         Name = "Призыватель " + (state == UserState.Joined ? "присоединился" : "вышел"),
                         IconUrl = sgUser.GetAvatarUrl()
                     })
                     .WithDescription($"Никнейм: {sgUser.Mention} ({sgUser.Username}#{sgUser.Discriminator})")
                     .WithFooter(new EmbedFooterBuilder
                     {
                         Text = $"ID: {sgUser.Id}"
                     })
                     .WithCurrentTimestamp();

            await usersChannel.SendEmbedAsync(eb);
        }

        async Task UserLeft(SocketGuildUser user)
        {
            await RegisterJoinedLeft(user, UserState.Left);
        }

        async Task<bool> HasCaps(string content)
        {
            string msg = new Regex(@"\<[^>]+>")
                         .Replace(content, "")
                         .Replace(" ", String.Empty)
                         .Replace("\t\n", String.Empty)
                         .Replace("\n", String.Empty)
                         .Replace(" ︀︀", String.Empty)
                         .Trim();

            if (msg.Length == 1)
                return false;


            List<char> upperCase = new List<char>();

            await Task.Run(delegate { upperCase = msg.Where(x => Char.IsLetter(x) && Char.IsUpper(x)).ToList(); });

            float ratio = (float) upperCase.Count() / (float) msg.Length;

            return ratio >= Settings.Chat.CapsFilterRatio;
        }

        const string YoutubeRegex =
            "^(?:https?\\:\\/\\/)?(?:www\\.)?(?:youtu\\.be\\/|youtube\\.com\\/(?:embed\\/|v\\/|watch\\?v\\=))([\\w-]{10,12})(?:[\\&\\?\\#].*?)*?(?:[\\&\\?\\#]t=([\\dhm]+s))?$";

        static bool HasURL(string message)
        {
            string link = message.Replace(" ", String.Empty)
                                 .Split("\t\n ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                                 .Where(s => s.Contains("http://")
                                             || s.Contains("www.")
                                             || s.Contains("https://")
                                             || s.Contains("ftp://"))
                                 .ToArray()
                                 .FirstOrDefault();

            if (String.IsNullOrWhiteSpace(link))
                return false;

            if (IsYoutubeLink(link))
                return !Settings.Chat.AllowYoutubeLinks;

            return true;
        }

        static Regex ytRegex = new Regex(YoutubeRegex);

        static bool IsYoutubeLink(string url)
        {
            return ytRegex.IsMatch(url);
        }

        static Dictionary<ulong, DateTime> lastPostTimestamps = new Dictionary<ulong, DateTime>();

        static bool IsEligibleForEconomy(ulong userId)
        {
            if (!lastPostTimestamps.ContainsKey(userId))
            {
                lastPostTimestamps.Add(userId, DateTime.UtcNow);

                return true;
            }

            var currentTime = DateTime.UtcNow;

            bool result = (currentTime - lastPostTimestamps[userId]) > Settings.Economy.MessageCooldown;

            if (result)
                lastPostTimestamps[userId] = currentTime;

            return result;
        }
    }

    public enum UserState
    {
        Joined,
        Left,
    }
}
