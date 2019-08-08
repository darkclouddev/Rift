using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Services;
using Rift.Services.Message;
using Rift.Util;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using IonicLib;
using IonicLib.Services;

using Microsoft.Extensions.DependencyInjection;

using NLog;

using ILogger = NLog.ILogger;

namespace Rift
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RiftBot
    {
        public static CultureInfo Culture { get; private set; }

        public static ILogger Log { get; private set; }

        public static string AppPath { get; private set; }

        public static string InternalVersion { get; private set; } = "6.0";
        public const string CommandDenyMessage = "У вас нет доступа к этой команде.";
        public static string BotStatusMessage => Settings.App.MaintenanceMode ? "Тестовый режим" : "Подготовка к запуску";

        public static bool ShouldReboot;

        static CommandHandler handler;

        static readonly List<ulong> adminIds = new List<ulong>
        {
            212997107525746690ul, //me
            178443743026872321ul, //wise
        };

        static readonly List<ulong> developersIds = new List<ulong>
        {
            212997107525746690ul, //me
        };

        public static bool IsAdmin(IUser user)
        {
            return adminIds.Contains(user.Id) || developersIds.Contains(user.Id);
        }

        public static async Task<bool> IsModeratorAsync(IUser user)
        {
            var role = await DB.Roles.GetAsync(43);
            
            return user is SocketGuildUser socketUser
                   && socketUser.Roles.Any(x => x.Id == role.RoleId);
        }

        public static bool IsDeveloper(IUser user)
        {
            return developersIds.Contains(user.Id);
        }

        public static T GetService<T>()
        {
            return handler.provider.GetService<T>();
        }

        public static async Task SendMessageToDevelopers(string msg)
        {
            foreach (var userId in developersIds)
            {
                var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

                if (sgUser is null)
                    return;

                await sgUser.SendMessageAsync(msg);
            }
        }

        public static async Task SendMessageToAdmins(string msg)
        {
            foreach (var userId in developersIds)
            {
                var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

                if (sgUser is null)
                    return;

                await sgUser.SendMessageAsync(msg);
            }
        }

        public static async Task<IonicMessage> GetMessageAsync(string identifier, FormatData data)
        {
            return await GetService<MessageService>().GetMessageAsync(identifier, data).ConfigureAwait(false);
        }

        public static async Task<IUserMessage> SendMessageAsync(string identifier, ulong channelId, FormatData data)
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, channelId, out var channel))
                return null;

            var msg = await GetService<MessageService>().GetMessageAsync(identifier, data);

            if (msg is null)
                return null;

            return await channel.SendIonicMessageAsync(msg).ConfigureAwait(false);
        }

        public static async Task<IUserMessage> SendMessageAsync(IonicMessage message, ulong channelId)
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, channelId, out var channel))
                return null;

            return await channel.SendIonicMessageAsync(message);
        }

        public static async Task Main(string[] args)
        {
            await Settings.ReloadAllAsync();
            AppPath = GetContentRoot();
            Culture = new CultureInfo("ru-RU")
            {
                DateTimeFormat = {Calendar = new GregorianCalendar()}
            };

            Console.WriteLine($"Using content root: {AppPath}");

            await new IonicClient(Path.Combine(AppPath, ".token"))
                .RunAsync()
                .ConfigureAwait(false);

            await new RiftBot()
                .RunAsync()
                .ConfigureAwait(false);
        }

        public static string GetContentRoot()
        {
            var assemblyPath = Assembly.GetEntryAssembly().Location;
            var fwdSlashIndex = assemblyPath.LastIndexOf('/'); // *nix
            var backSlashIndex = assemblyPath.LastIndexOf('\\'); // windows
            var index = Math.Max(fwdSlashIndex, backSlashIndex);

            return assemblyPath.Substring(0, index);
        }

        public async Task RunAsync()
        {
            LogManager.LoadConfiguration("nlog.config");
            Log = LogManager.GetCurrentClassLogger();

            var serviceProvider = SetupServices();

            handler = new CommandHandler(serviceProvider);
            await handler.ConfigureAsync();

            try
            {
                await Task.Delay(-1, IonicClient.TokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                await IonicClient.Client.StopAsync().ContinueWith(x =>
                {
                    if (!ShouldReboot)
                    {
                        Console.WriteLine("Shutting down");
                        LogManager.Shutdown();
                    }
                    else
                    {
                        Console.WriteLine("Restarting...");
                        LogManager.Shutdown();
                        Environment.Exit(4); //anything higher than zero is a restart
                    }
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        static IServiceProvider SetupServices()
        {
            var services = new ServiceCollection()
                .AddSingleton(Log)
                .AddSingleton(IonicClient.Client)
                .AddSingleton(new EconomyService())
                .AddSingleton(new GiftService())
                .AddSingleton(new RoleService())
                .AddSingleton(new RiotService())
                .AddSingleton(new EmoteService())
                .AddSingleton(new MessageService())
                .AddSingleton(new ToxicityService())
                .AddSingleton(new GiveawayService())
                .AddSingleton(new BragService())
                .AddSingleton(new EventService())
                .AddSingleton(new BotRespectService())
                .AddSingleton(new QuizService())
                .AddSingleton(new ModerationService())
                .AddSingleton(new ReliabilityService(IonicClient.Client))
                .AddSingleton(new ChannelService(IonicClient.Client))
                .AddSingleton(new QuestService())
                .AddSingleton(new StoreService())
                .AddSingleton(new ChestService())
                .AddSingleton(new CapsuleService())
                .AddSingleton(new SphereService())
                .AddSingleton(new BackgroundService())
                .AddSingleton(new VoteService())
                .AddSingleton(new RewindService())
                .AddSingleton(new DoubleExpService())
                .AddSingleton(new DailyService())
                .AddSingleton(new RoleSetupService(IonicClient.Client))
                .AddSingleton(new CommandService(new CommandServiceConfig
                {
                    CaseSensitiveCommands = false,
                    ThrowOnError = true,
                    DefaultRunMode = RunMode.Async
                }));

            return services.BuildServiceProvider();
        }
    }
}
