using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Humanizer;

using IonicLib;
using IonicLib.Extensions;

using Rift.Database;
using Rift.Events;
using Rift.Services.Message;
using Rift.Services.Reward;

using Settings = Rift.Configuration.Settings;

namespace Rift.Services
{
    public class BotRespectService
    {
        public static event EventHandler<ActivatedBotRespectsEventArgs> ActivatedBotRespects;
        
        Timer timer;
        readonly Timer Starttimer;

        static readonly List<ItemReward> AvailableRewards = new List<ItemReward>
        {
            new ItemReward().AddChests(1),
            new ItemReward().AddTickets(2),
            new ItemReward().AddChests(2),
            new ItemReward().AddCoins(1000),
            new ItemReward().AddDoubleExps(1),
            new ItemReward().AddTokens(1),
            new ItemReward().AddChests(3),
            new ItemReward().AddCoins(500),
        };

        public BotRespectService()
        {
            /*RiftBot.Log.Info("Starting BotRespectService..");
            
            timer = new Timer(
                async delegate { await StartBotGifts(); },
                null,
                Timeout.Infinite,
                0);
            
            InitTimer();

            RiftBot.Log.Info("BotRespectService loaded successfully.");*/
        }

        void InitTimer()
        {
            var ts = TimeSpan.FromSeconds(Helper.NextInt(210, 330) * 60);
            timer.Change(ts, TimeSpan.Zero);

            RiftBot.Log.Debug($"Next gifts: {ts.Humanize()}");
        }

        async Task StartBotGifts()
        {
            RiftBot.Log.Debug("Gifts are on the way..");

            var users = await DB.Cooldowns.GetBotRespectedUsersAsync();

            if (users.Count == 0)
            {
                RiftBot.Log.Debug("No users with bot respect, rescheduling.");
                InitTimer();
                return;
            }
            
            foreach (var userId in users)
            {
                var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);
                
                if (sgUser is null)
                    continue;

                var reward = AvailableRewards.Random();
                await reward.DeliverToAsync(userId);
            }

            await RiftBot.SendMessageAsync("botrespect-success", Settings.ChannelId.Chat, null);

            RiftBot.Log.Debug("Finished sending gifts");

            InitTimer();
        }
        
        public async Task ActivateAsync(ulong userId)
        {
            var dbInventory = await DB.Inventory.GetAsync(userId);

            if (dbInventory.BonusBotRespect == 0)
            {
                await RiftBot.SendMessageAsync("activate-nopowerup", Settings.ChannelId.Comms, new FormatData(userId));
                return;
            }

            var dbCooldowns = await DB.Cooldowns.GetAsync(userId);
            if (dbCooldowns.BotRespectTime > DateTime.UtcNow)
            {
                await RiftBot.SendMessageAsync("activate-active", Settings.ChannelId.Comms, new FormatData(userId));
                return;
            }

            await DB.Inventory.RemoveAsync(userId, new InventoryData {BotRespects = 1});
            ActivatedBotRespects?.Invoke(null, new ActivatedBotRespectsEventArgs(userId));

            var dateTime = DateTime.UtcNow.AddHours(12.0);
            await DB.Cooldowns.SetBotRespeсtTimeAsync(userId, dateTime);

            await RiftBot.SendMessageAsync("activate-success-botrespect", Settings.ChannelId.Comms, new FormatData(userId));
        }
    }
}
