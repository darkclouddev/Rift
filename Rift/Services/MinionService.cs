using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Services.Message;
using Rift.Services.Reward;
using Rift.Util;

using IonicLib;
using IonicLib.Extensions;

namespace Rift.Services
{
    public class MinionService
    {
        static readonly TimeSpan MinionLifeTime = TimeSpan.FromMinutes(5);

        static readonly List<(uint, ItemReward)> AvailableRewards = new List<(uint, ItemReward)>
        {
            (100, new ItemReward().AddCoins(Helper.NextUInt(100, 501)))
        };

        Timer StartupTimer;
        Timer MinionTimer;
        Timer MinionSuicideTimer;
        Timer MinionKillTimer;
        ulong killerId;

        MinionColor minionColor;
        string minionImage;

        static List<string> RedMinionImages = new List<string>
        {
            "https://cdn.discordapp.com/attachments/476084342872080385/476085040434905091/2223.png",
            "https://cdn.discordapp.com/attachments/476084342872080385/476085042137792571/2226.png"
        };

        static List<string> BlueMinionImages = new List<string>
        {
            "https://cdn.discordapp.com/attachments/476084342872080385/476085039319351317/2224.png",
            "https://cdn.discordapp.com/attachments/476084342872080385/476085040472653835/2225.png"
        };

        public ulong KillerId
        {
            get => killerId;
            set
            {
                if (killerId == 0)
                    killerId = value;
            }
        }

        public MinionService()
        {
            RiftBot.Log.Info("Starting MinionService..");

            StartupTimer = new Timer(
                async delegate { await SetupNextMinionAsync(); },
                null,
                TimeSpan.FromSeconds(30),
                TimeSpan.Zero);

            MinionKillTimer = new Timer(
                async delegate { await KillMinion(); },
                null,
                Timeout.Infinite,
                0);

            MinionSuicideTimer = new Timer(
                async delegate { await Suicide(); },
                null,
                Timeout.Infinite,
                0);

            MinionTimer = new Timer(
                async delegate { await SpawnMinion(); },
                null,
                Timeout.Infinite,
                0);

            RiftBot.Log.Info("MinionService loaded successfully.");
        }

        void DropTimers()
        {
            MinionKillTimer.Change(Timeout.Infinite, 0);
            MinionSuicideTimer.Change(Timeout.Infinite, 0);
        }

        public Task SetupNextMinionAsync(uint minionTs = uint.MaxValue)
        {
            DropTimers();

            if (Helper.NextUInt(0, 2) % 2 == 0)
            {
                minionColor = MinionColor.Blue;
                minionImage = BlueMinionImages.Random();
            }
            else
            {
                minionImage = RedMinionImages.Random();
                minionColor = MinionColor.Red;
            }

            if (minionTs == uint.MaxValue) // default value
                minionTs = Helper.NextUInt(60, 90); // 1 - 1.5 hours

            minionTs *= 60;

            RiftBot.Log.Debug(nameof(MinionService),
                              $"Next Minion: {Helper.FromTimestamp(Helper.CurrentUnixTimestamp + minionTs).ToString()}");

            MinionTimer.Change(TimeSpan.FromSeconds(minionTs), TimeSpan.Zero);

            return Task.CompletedTask;
        }

        public async Task SpawnMinion()
        {
            killerId = 0;

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Comms, out var channel))
                return;

            RiftBot.Log.Debug("Minion spawned");

            var msgMinionSpawned = await RiftBot.GetMessageAsync("minion-spawned", null);
            var msg = await channel.SendIonicMessageAsync(msgMinionSpawned);
            RiftBot.GetService<MessageService>().TryAddDelete(new DeleteMessage(msg, TimeSpan.FromHours(1)));

            MinionKillTimer.Change(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
            MinionSuicideTimer.Change(MinionLifeTime, TimeSpan.Zero);
        }

        public async Task KillMinion()
        {
            if (killerId == 0)
                return;

            DropTimers();
            await AvailableRewards.Random().Item2.DeliverToAsync(killerId);

            RiftBot.Log.Debug($"Minion was killed by {killerId.ToString()}");

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Comms, out var channel))
                return;

            var msgMinionKilled = await RiftBot.GetMessageAsync("minion-killed", new FormatData(killerId)
            {
            });
            //await channel.SendEmbedAsync(MinionEmbeds.MinionKilled(minionColor, killerId, reward));
            await channel.SendIonicMessageAsync(msgMinionKilled);
            await SetupNextMinionAsync();
        }

        public async Task Suicide()
        {
            DropTimers();

            RiftBot.Log.Debug("Minion suicide");

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Comms, out var channel))
                return;

            var msgMinionSuicide = await RiftBot.GetMessageAsync("minion-suicide", null);
            await channel.SendIonicMessageAsync(msgMinionSuicide);
            await SetupNextMinionAsync();
        }

        public void SetUpKiller(ulong userId)
        {
            KillerId = userId;
        }
    }

    public enum MinionColor
    {
        Red,
        Blue
    }
}
