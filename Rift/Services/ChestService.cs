using System;
using System.Threading;
using System.Threading.Tasks;

using Rift.Database;
using Rift.Events;
using Rift.Services.Interfaces;
using Rift.Services.Message;
using Rift.Services.Reward;

using Settings = Rift.Configuration.Settings;

namespace Rift.Services
{
    public class ChestService : IChestService
    {
        public event EventHandler<ChestsOpenedEventArgs> ChestsOpened;

        readonly IMessageService messageService;
        readonly IRewardService rewardService;
        
        static readonly SemaphoreSlim Mutex = new SemaphoreSlim(1);

        public ChestService(IMessageService messageService,
                            IRewardService rewardService)
        {
            this.messageService = messageService;
            this.rewardService = rewardService;
        }
        
        public async Task OpenAsync(ulong userId, uint amount)
        {
            await Mutex.WaitAsync().ConfigureAwait(false);

            RiftBot.Log.Information($"Opening {amount.ToString()} chest(s) for {userId.ToString()}.");

            try
            {
                await OpenInternalAsync(userId, amount).ConfigureAwait(false);
            }
            finally
            {
                Mutex.Release();
            }
        }

        public async Task OpenAllAsync(ulong userId)
        {
            await Mutex.WaitAsync().ConfigureAwait(false);

            try
            {
                var dbInventory = await DB.Inventory.GetAsync(userId);
                await OpenInternalAsync(userId, dbInventory.Chests).ConfigureAwait(false);
            }
            finally
            {
                Mutex.Release();
            }
        }

        async Task OpenInternalAsync(ulong userId, uint amount)
        {
            var dbInventory = await DB.Inventory.GetAsync(userId);

            if (dbInventory.Chests == 0)
            {
                await messageService.SendMessageAsync("chests-nochests", Settings.ChannelId.Commands, new FormatData(userId));
                return;
            }

            if (dbInventory.Chests < amount || amount == 0)
            {
                await messageService.SendMessageAsync("chests-notenoughchests", Settings.ChannelId.Commands, new FormatData(userId));
                return;
            }

            await DB.Inventory.RemoveAsync(userId, new InventoryData {Chests = amount});
            ChestsOpened?.Invoke(null, new ChestsOpenedEventArgs(userId, amount));

            var reward = new ChestReward(amount);
            await rewardService.DeliverToAsync(userId, reward);
            await DB.Statistics.AddAsync(userId, new StatisticData {ChestsOpened = amount});

            await messageService.SendMessageAsync("chests-open-success", Settings.ChannelId.Commands, new FormatData(userId)
            {
                Reward = reward
            });
        }
    }
}
