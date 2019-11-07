using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
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

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

using Rift.Data;
using Rift.Services.Interfaces;

using Serilog;
using Serilog.Events;

namespace Rift
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RiftBot
    {
        public static CultureInfo Culture { get; private set; }

        public static ILogger Log { get; private set; }
        public static ILogger ChatLogger { get; set; }
        static ILogger DiscordLogger { get; set; }

        public static string AppPath { get; private set; }

        public static string InternalVersion { get; private set; } = "6.0";
        public const string CommandDenyMessage = "У вас нет доступа к этой команде.";
        public static string BotStatusMessage => Settings.App.MaintenanceMode ? "Тестовый режим" : "!команды";

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

        public static async Task SendMessageToDevelopers(string msg)
        {
            foreach (var userId in developersIds)
            {
                if (!IonicHelper.GetGuildUserById(Settings.App.MainGuildId, userId, out var sgUser))
                    return;

                await sgUser.SendMessageAsync(msg);
            }
        }

        public static async Task SendMessageToAdmins(string msg)
        {
            foreach (var userId in developersIds)
            {
                if (!IonicHelper.GetGuildUserById(Settings.App.MainGuildId, userId, out var sgUser))
                    return;

                await sgUser.SendMessageAsync(msg);
            }
        }

        class MessageDto
        {
            public ulong GuildId { get; set; }
            public string Name { get; set; }
            public string Text { get; set; }
            public string EmbedJson { get; set; }
            public string ImageUrl { get; set; }
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

            var tokenData = await File.ReadAllTextAsync(Path.Combine(AppPath, ".token"));

            await new IonicHelper(tokenData)
                .RunAsync(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Warning,
                    MessageCacheSize = 1000
                }, LogDiscord )
                .ConfigureAwait(false);

            await RunAsync()
                .ConfigureAwait(false);
        }

        static Task LogDiscord(LogMessage arg)
        {
            var text = arg.Message;
            
            switch (arg.Severity)
            {
                case LogSeverity.Critical:
                    DiscordLogger.Write(LogEventLevel.Fatal, text);
                    break;
                
                case LogSeverity.Error:
                    DiscordLogger.Write(LogEventLevel.Error, text);
                    break;
                
                case LogSeverity.Warning:
                    DiscordLogger.Write(LogEventLevel.Warning, text);
                    break;
                
                case LogSeverity.Info:
                    DiscordLogger.Write(LogEventLevel.Information, text);
                    break;
                
                case LogSeverity.Debug:
                    DiscordLogger.Write(LogEventLevel.Debug, text);
                    break;
                
                case LogSeverity.Verbose:
                default:
                    DiscordLogger.Write(LogEventLevel.Verbose, text);
                    break;
            }
            
            return Task.CompletedTask;
        }

        static string GetContentRoot()
        {
            var assemblyPath = Assembly.GetEntryAssembly().Location;
            var fwdSlashIndex = assemblyPath.LastIndexOf('/'); // *nix
            var backSlashIndex = assemblyPath.LastIndexOf('\\'); // windows
            var index = Math.Max(fwdSlashIndex, backSlashIndex);

            return assemblyPath.Substring(0, index);
        }

        static async Task RunAsync()
        {
            Log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Async(x => x.File("logs/rift-.log", rollingInterval: RollingInterval.Day))
                .CreateLogger();

            DiscordLogger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Async(x => x.File("logs/discord-.log", rollingInterval: RollingInterval.Day))
                .CreateLogger();

            ChatLogger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Async(x => x.File("logs/chat-.log", rollingInterval: RollingInterval.Day))
                .CreateLogger();

            var serviceProvider = SetupServices();

            handler = new CommandHandler(serviceProvider);
            await handler.ConfigureAsync();

            try
            {
                await Task.Delay(-1, IonicHelper.GetCancellationTokenSource().Token);
            }
            catch (TaskCanceledException)
            {
                await IonicHelper.Client.StopAsync().ContinueWith(x =>
                {
                    if (!ShouldReboot)
                    {
                        Console.WriteLine("Shutting down");
                        Serilog.Log.CloseAndFlush();
                    }
                    else
                    {
                        Console.WriteLine("Restarting...");
                        Serilog.Log.CloseAndFlush();
                        Environment.Exit(4); //anything higher than zero is a restart
                    }
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occured while shutting down");
            }
        }
        
        public static IServiceProvider GetServiceProvider() => handler.Provider;

        static IServiceProvider SetupServices()
        {
            var services = new ServiceCollection()
                .AddSingleton(Log)
                .AddSingleton(IonicHelper.Client)
                .AddSingleton(typeof(IEconomyService), typeof(EconomyService))
                .AddSingleton(typeof(IGiftService), typeof(GiftService))
                .AddSingleton(typeof(IRoleService), typeof(RoleService))
                .AddSingleton(typeof(IRiotService), typeof(RiotService))
                .AddSingleton(typeof(IEmoteService), typeof(EmoteService))
                .AddSingleton(typeof(IMessageService), typeof(MessageService))
                .AddSingleton(typeof(IToxicityService), typeof(ToxicityService))
                .AddSingleton(typeof(IGiveawayService), typeof(GiveawayService))
                .AddSingleton(typeof(IBragService), typeof(BragService))
                .AddSingleton(typeof(IEventService), typeof(EventService))
                .AddSingleton(typeof(IBotRespectService), typeof(BotRespectService))
                .AddSingleton(typeof(IModerationService), typeof(ModerationService))
                .AddSingleton(typeof(IChannelService), typeof(ChannelService))
                .AddSingleton(typeof(IStoreService), typeof(StoreService))
                .AddSingleton(typeof(IChestService), typeof(ChestService))
                .AddSingleton(typeof(ICapsuleService), typeof(CapsuleService))
                .AddSingleton(typeof(ISphereService), typeof(SphereService))
                .AddSingleton(typeof(IBackgroundService), typeof(BackgroundService))
                .AddSingleton(typeof(IRewindService), typeof(RewindService))
                .AddSingleton(typeof(IDoubleExpService), typeof(DoubleExpService))
                .AddSingleton(typeof(IDailyService), typeof(DailyService))
                .AddSingleton(typeof(IRoleSetupService), typeof(RoleSetupService))
                .AddSingleton(typeof(IRewardService), typeof(RewardService))
                .AddSingleton(new ReliabilityService(IonicHelper.Client))
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
