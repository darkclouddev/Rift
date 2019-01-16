using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Services;

using IonicLib;
using IonicLib.Services;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using NLog;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

using ILogger = NLog.ILogger;

namespace Rift
{
    public class RiftBot
    {
        public static CultureInfo Culture { get; private set; }

        public static ILogger Log { get; private set; }

        public static string AppPath { get; private set; }

        public static string InternalVersion { get; private set; } = "5.0";
        public const string CommandDenyMessage = "У вас нет доступа к этой команде.";
        public static string BotStatusMessage => Settings.App.MaintenanceMode ? "Тестовый режим" : "!команды";

        public static bool IsReboot;

        static CommandHandler handler;

        const string revisionFileName = "revision";

        static readonly List<Streamer> streamers = new List<Streamer>
		{
			new Streamer(
				119837878225338370ul, //fantastic тащ <3
				"https://cdn.discordapp.com/attachments/365934510153662464/384016832534937612/1.png",
				StreamPlatform.Twitch,
				"https://www.twitch.tv/fantastictouch/"),
			new Streamer(
				232221573757534208ul, //teynor
				"https://cdn.discordapp.com/attachments/365934510153662464/384036041599877121/134.png",
				StreamPlatform.Youtube,
				"https://www.youtube.com/c/TEYNOR/live"),
			new Streamer(
				247422043308556300ul, //offiners
				"https://cdn.discordapp.com/attachments/365934510153662464/384037902763556864/134.png",
				StreamPlatform.Twitch,
				"https://www.twitch.tv/offiners"),

            new Streamer(
                112194571705786368ul, //jestkuimax
				"https://cdn.discordapp.com/attachments/503168681333882891/513333717314568223/logo.png",
                StreamPlatform.Twitch,
                "https://www.twitch.tv/jestkuimax"),
            new Streamer(
                164427344889511937ul, //nomanzzz
				"https://cdn.discordapp.com/attachments/503168681333882891/513333717314568223/logo.png",
                StreamPlatform.Twitch,
                "https://www.twitch.tv/nomanzzz"),
            new Streamer(
                114315170175516673ul, //корпорация призывателей
				"https://cdn.discordapp.com/attachments/503168681333882891/513333717314568223/logo.png",
                StreamPlatform.Twitch,
                "https://www.twitch.tv/summonersinc"),
            new Streamer(
                178912866617786375ul, //lekcycc
				"https://cdn.discordapp.com/attachments/503168681333882891/513333717314568223/logo.png",
                StreamPlatform.Twitch,
                "https://www.twitch.tv/lekcycc"),
             new Streamer(
                178443743026872321ul, //wise
				"https://cdn.discordapp.com/attachments/503168681333882891/513333717314568223/logo.png",
                StreamPlatform.Twitch,
                "https://www.twitch.tv/leaguewise"),
        };

        public class Streamer
        {
            public ulong Id { get; set; }
            public string PictureURL { get; set; }
            public StreamPlatform Platform { get; set; }
            public string StreamURL { get; set; }

            public Streamer(ulong id, string picUrl, StreamPlatform platform, string streamUrl)
            {
                Id = id;
                PictureURL = picUrl;
                Platform = platform;
                StreamURL = streamUrl;
            }
        }

        public enum StreamPlatform
        {
            Twitch,
            Youtube
        }

		static readonly List<ulong> adminIds = new List<ulong>
		{
            212997107525746690ul, //me
			178443743026872321ul, //wise
        };

		static readonly List<ulong> developersIds = new List<ulong>
		{
			212997107525746690ul, //me
        };

		public static Streamer GetStreamer(ulong userId)
		{
			return streamers.FirstOrDefault(x => x.Id == userId);
		}

		public static bool IsAdmin(IUser user) => adminIds.Contains(user.Id) || developersIds.Contains(user.Id);

		public static bool IsModerator(IUser user)
		{
			return (user is SocketGuildUser socketUser && socketUser.Roles.Any(x => x.Id == Settings.RoleId.Moderator || x.Id == Settings.RoleId.BossModerator));
		}

		public static bool IsStreamer(ulong userId)
		{
			return streamers.Any(x => x.Id == userId);
		}

		public static bool IsDeveloper(IUser user) => developersIds.Contains(user.Id);

		public static T GetService<T>() => handler.provider.GetService<T>();

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

        public static async Task Main(string[] args)
        {
            AppPath = GetContentRoot();
            Culture = new CultureInfo("ru-RU");

            Console.WriteLine($"Using content root: {AppPath}");

            //await CreateWebHostBuilder(args).Build().StartAsync();

            await new IonicClient(Path.Combine(AppPath, ".token"), Culture, new GregorianCalendar())
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

            Log.Info($"Using content root: {AppPath}");

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
                    if (!IsReboot)
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
                           .AddSingleton(new DatabaseService())
                           .AddSingleton(IonicClient.Client)
                           .AddSingleton(new EconomyService())
                           .AddSingleton(new RoleService())
                           .AddSingleton(new RiotService())
                           .AddSingleton(new MessageService())
                           .AddSingleton(new GiveawayService())
                           .AddSingleton(new AnnounceService())
                           .AddSingleton(new EventService())
                           .AddSingleton(new MinionService())
                           .AddSingleton(new BotRespectService())
                           .AddSingleton(new ReliabilityService(IonicClient.Client))
                           //.AddSingleton(new ChannelService(IonicClient.Client))
                           .AddSingleton(new CommandService(new CommandServiceConfig
                           {
                               CaseSensitiveCommands = false,
                               ThrowOnError = true,
                               DefaultRunMode = RunMode.Async
                           }));

            var provider = services.BuildServiceProvider();

            return provider;
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
