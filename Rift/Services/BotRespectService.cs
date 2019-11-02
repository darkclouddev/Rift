using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Humanizer;

using IonicLib;
using IonicLib.Extensions;

using Rift.Database;
using Rift.Events;
using Rift.Services.Interfaces;
using Rift.Services.Message;
using Rift.Services.Reward;

using Settings = Rift.Configuration.Settings;

namespace Rift.Services
{
    public class BotRespectService : IBotRespectService
    {
        public event EventHandler<ActivatedBotRespectsEventArgs> ActivatedBotRespects;

        readonly IMessageService messageService;
        readonly IRewardService rewardService;
        readonly Timer timer;

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

        public BotRespectService(IMessageService messageService,
                                 IRewardService rewardService)
        {
            RiftBot.Log.Information($"Starting {nameof(BotRespectService)}..");

            this.messageService = messageService;
            this.rewardService = rewardService;
            
            timer = new Timer(
                async delegate { await StartBotGifts(); },
                null,
                Timeout.Infinite,
                0);
            
            InitTimer();

            RiftBot.Log.Information($"{nameof(BotRespectService)} loaded successfully.");
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
                if (!IonicHelper.GetGuildUserById(Settings.App.MainGuildId, userId, out var sgUser))
                    continue;

                var reward = AvailableRewards.Random();
                await rewardService.DeliverToAsync(userId, reward);
            }

            await messageService.SendMessageAsync("yasuo-botrespect-success", Settings.ChannelId.Chat, null);

            RiftBot.Log.Debug("Finished sending gifts");

            InitTimer();
        }
        
        public async Task ActivateAsync(ulong userId)
        {
            var dbInventory = await DB.Inventory.GetAsync(userId);
            if (dbInventory.BonusBotRespect == 0)
            {
                await messageService.SendMessageAsync("bonus-nobonus", Settings.ChannelId.Commands, new FormatData(userId));
                return;
            }

            var dbCooldowns = await DB.Cooldowns.GetAsync(userId);
            if (dbCooldowns.BotRespectTime > DateTime.UtcNow)
            {
                await messageService.SendMessageAsync("bonus-active", Settings.ChannelId.Commands, new FormatData(userId));
                return;
            }

            await DB.Inventory.RemoveAsync(userId, new InventoryData {BotRespects = 1});
            ActivatedBotRespects?.Invoke(null, new ActivatedBotRespectsEventArgs(userId));

            var dateTime = DateTime.UtcNow.AddHours(12.0);
            await DB.Cooldowns.SetBotRespeсtTimeAsync(userId, dateTime);

            await messageService.SendMessageAsync("botrespect-success", Settings.ChannelId.Commands, new FormatData(userId));
        }
    }
}
