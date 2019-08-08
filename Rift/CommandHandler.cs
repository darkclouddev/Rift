using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;

using Settings = Rift.Configuration.Settings;

using Rift.Database;
using Rift.Services;
using Rift.Services.Message;
using Rift.Util;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using IonicLib;
using IonicLib.Util;

using Microsoft.Extensions.DependencyInjection;

namespace Rift
{
    public class CommandHandler
    {
        const char CommandPrefix = '!';

        public readonly IServiceProvider provider;
        static DiscordSocketClient client;
        static CommandService commandService;
        static EconomyService economyService;
        static RoleService roleService;
        static RiotService riotService;
        static MessageService messageService;
        static QuizService quizService;
        static EmoteService emoteService;
        static QuestService questService;
        static DailyService dailyService;

        public CommandHandler(IServiceProvider serviceProvider)
        {
            provider = serviceProvider;

            client = provider.GetService<DiscordSocketClient>();
            commandService = provider.GetService<CommandService>();
            economyService = provider.GetService<EconomyService>();
            roleService = provider.GetService<RoleService>();
            messageService = provider.GetService<MessageService>();
            riotService = provider.GetService<RiotService>();
            quizService = provider.GetService<QuizService>();
            emoteService = provider.GetService<EmoteService>();
            questService = provider.GetService<QuestService>();
            dailyService = provider.GetService<DailyService>();
        }

        public async Task ConfigureAsync()
        {
            await commandService.AddModulesAsync(Assembly.GetEntryAssembly(), provider);
            economyService.Init();

            await riotService.InitAsync();

            client.MessageReceived += ProcessMessage;
            client.UserJoined += UserJoined;
            client.UserLeft += UserLeft;
            client.Ready += ReadyAsync;
        }

        static async Task ReadyAsync()
        {
            await emoteService.ReloadEmotesAsync();
            await client.SetGameAsync(RiftBot.BotStatusMessage);
        }

        async Task ProcessMessage(SocketMessage socketMsg)
        {
            if (!(socketMsg is SocketUserMessage message))
                return;

            if (message.Author.IsBot)
                return;

            if (Settings.App.MaintenanceMode && !RiftBot.IsDeveloper(message.Author)
                && !RiftBot.IsAdmin(message.Author)
                && !await RiftBot.IsModeratorAsync(message.Author))
                return;

            if (await DB.Toxicity.HasBlockingAsync(message.Author.Id))
                return;
            
            var context = new CommandContext(client, message);

            if (context.IsPrivate)
                return;
            
            if (await HandleCommand(context))
                return;

            await HandlePlainText(context);
        }

        async Task<bool> HandleCommand(CommandContext context)
        {
            var argPos = 0;

            if (context.Message.HasCharPrefix(CommandPrefix, ref argPos))
            {
                if (context.Channel.Id != Settings.ChannelId.Commands)
                    return true;
                
                var result = await commandService.ExecuteAsync(context, argPos, provider);

                if (result.IsSuccess)
                {
                    var command = context.Message.Content.Substring(1).TrimStart();
                    RiftBot.Log.Info($"{context.Message.Author} sent channel command: \"{command}\"");
                }
                else
                {
                    if (result.Error != null && result.Error.Value != CommandError.UnknownCommand)
                        RiftBot.Log.Error(result.ErrorReason);

                    if (result.Error != null)
                    {
                        string errorMessage;
                        
                        var error = result.Error.Value;

                        switch (error)
                        {
                            case CommandError.UnknownCommand:
                                errorMessage = "Неизвестная команда!";
                                break;

                            case CommandError.BadArgCount:
                            case CommandError.ParseFailed:
                                errorMessage = "Неверные параметры команды!";
                                break;

                            default:
                                errorMessage = result.ErrorReason;
                                break;
                        }

                        await context.Channel.SendMessageAsync($"<@{context.Message.Author.Id.ToString()}> {errorMessage}");
                    }
                }

                //messageService.TryAddDelete(new DeleteMessage(context.Message, TimeSpan.FromSeconds(5)));
                return true;
            }

            return false;
        }

        static async Task HandlePlainText(CommandContext context)
        {
            if (context.Message.Channel.Id != Settings.ChannelId.Chat)
                return;

            //if (quizService.IsActive) // TODO: uncomment when quiz will be ready
            //    quizService.Answers.Enqueue(new QuizGuess(context.User.Id, context.Message.Content));

            if (Settings.Chat.AttachmentFilterEnabled && !RiftBot.IsAdmin(context.Message.Author))
                if (context.Message.Attachments != null && context.Message.Attachments.Count > 0)
                    await context.Message.DeleteAsync();

            if (Settings.Chat.CapsFilterEnabled && await HasCaps(context.Message.Content))
            {
                await context.Message.DeleteAsync();

                var eb = new EmbedBuilder()
                    .WithDescription($":no_entry_sign: {context.Message.Author.Mention} капсить на сервере запрещено!");

                await context.User.SendEmbedAsync(eb);
                return;
            }

            if (Settings.Chat.UrlFilterEnabled && HasUrl(context.Message.Content))
            {
                await context.Message.DeleteAsync();

                var eb = new EmbedBuilder()
                    .WithDescription($":no_entry_sign: {context.Message.Author.Mention} ссылки на сервере запрещены!");

                await context.User.SendEmbedAsync(eb);
                return;
            }

            if (Settings.Chat.ProcessUserNames) // TODO: experimental feature
                if (context.User is SocketGuildUser sgUser)
                {
                    var name = string.IsNullOrWhiteSpace(sgUser.Nickname) ? sgUser.Username : sgUser.Nickname;

                    if (IsIncorrectName(name, out var editedName))
                    {
                        if (string.IsNullOrWhiteSpace(editedName))
                            editedName = $"tempname{DateTime.UtcNow.Millisecond.ToString()}";

                        await sgUser.ModifyAsync(x => { x.Nickname = editedName; });
                    }
                }

            await DB.Statistics.AddAsync(context.User.Id, new StatisticData {MessagesSent = 1u});
            await questService.TryAddFirstQuestAsync(context.User.Id);

            if (IsEligibleForEconomy(context.User.Id))
            {
                await dailyService.CheckAsync(context.User.Id);
                await EconomyService.ProcessMessageAsync(context.Message).ConfigureAwait(false);
            }
        }

        static async Task UserJoined(SocketGuildUser user)
        {
            if (user.Guild.Id != Settings.App.MainGuildId)
                return;

            await RegisterJoinedLeft(user, UserState.Joined);

            await RiftBot.SendMessageAsync("user-joined", Settings.ChannelId.Chat, new FormatData(user.Id));
        }

        static async Task RegisterJoinedLeft(SocketGuildUser sgUser, UserState state)
        {
            if (state == UserState.Joined)
            {
                await roleService.RestoreTempRolesAsync(sgUser);

                if (sgUser.Guild.Id == Settings.App.MainGuildId)
                {
                    var msg = await RiftBot.GetMessageAsync("welcome", null);
                    await sgUser.SendIonicMessageAsync(msg);
                }
            }

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Logs, out var usersChannel))
                return;

            var eb = new RiftEmbed()
                     .WithColor(state == UserState.Joined
                                    ? new Color(46, 204, 113)
                                    : new Color(231, 76, 60))
                     .WithAuthor("Призыватель " + (state == UserState.Joined ? "присоединился" : "вышел"),
                                 sgUser.GetAvatarUrl())
                     .WithDescription($"Никнейм: {sgUser.Mention} ({sgUser.Username}#{sgUser.Discriminator})")
                     .WithFooter($"ID: {sgUser.Id.ToString()}")
                     .WithCurrentTimestamp();

            await usersChannel.SendIonicMessageAsync(new IonicMessage(eb));
        }

        static async Task UserLeft(SocketGuildUser user)
        {
            await RegisterJoinedLeft(user, UserState.Left);
        }

        static async Task<bool> HasCaps(string content)
        {
            var msg = new Regex(@"\<[^>]+>")
                      .Replace(content, "")
                      .Replace(" ", string.Empty)
                      .Replace("\t\n", string.Empty)
                      .Replace("\n", string.Empty)
                      .Replace(" ︀︀", string.Empty)
                      .Trim();

            if (msg.Length == 1)
                return false;

            var upperCase = new List<char>();

            await Task.Run(delegate { upperCase = msg.Where(x => char.IsLetter(x) && char.IsUpper(x)).ToList(); });

            var ratio = upperCase.Count / (float) msg.Length;

            return ratio >= Settings.Chat.CapsFilterRatio;
        }

        const string YoutubeRegex =
            "^(?:https?\\:\\/\\/)?(?:www\\.)?(?:youtu\\.be\\/|youtube\\.com\\/(?:embed\\/|v\\/|watch\\?v\\=))([\\w-]{10,12})(?:[\\&\\?\\#].*?)*?(?:[\\&\\?\\#]t=([\\dhm]+s))?$";

        static readonly Regex YtRegex = new Regex(YoutubeRegex);

        static bool IsYoutubeLink(string url)
        {
            return YtRegex.IsMatch(url);
        }

        static bool HasUrl(string message)
        {
            var link = message.Replace(" ", string.Empty)
                              .Split("\t\n ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                              .Where(s => s.Contains("http://")
                                          || s.Contains("www.")
                                          || s.Contains("https://")
                                          || s.Contains("ftp://"))
                              .ToArray()
                              .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(link))
                return false;

            if (IsYoutubeLink(link))
                return !Settings.Chat.AllowYoutubeLinks;

            return true;
        }

        static Regex nameRegex = new Regex(@"^\p{L}[\p{L}\p{N}]*$");

        static bool IsIncorrectName(string name, out string editedName)
        {
            editedName = new string(name.Where(x => char.IsLetterOrDigit(x) || char.IsWhiteSpace(x) || x == ' ')
                                        .ToArray());

            return !name.Equals(editedName);
        }

        static readonly Dictionary<ulong, DateTime> LastPostTimestamps = new Dictionary<ulong, DateTime>();

        static bool IsEligibleForEconomy(ulong userId)
        {
            if (!LastPostTimestamps.ContainsKey(userId))
            {
                LastPostTimestamps.Add(userId, DateTime.UtcNow);
                return true;
            }

            var currentTime = DateTime.UtcNow;
            var isEligible = currentTime - LastPostTimestamps[userId] > Settings.Economy.MessageCooldown;

            if (isEligible)
                LastPostTimestamps[userId] = currentTime;

            return isEligible;
        }
    }

    public enum UserState
    {
        Joined,
        Left,
    }
}
