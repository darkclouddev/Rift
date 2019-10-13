using System;
using System.Threading;
using System.Threading.Tasks;

using Rift.Database;
using Rift.Events;
using Rift.Services.Message;
using Rift.Services.Reward;

using Settings = Rift.Configuration.Settings;

namespace Rift.Services
{
    public class ChestService
    {
        public static event EventHandler<ChestsOpenedEventArgs> ChestsOpened;
        
        static readonly SemaphoreSlim Mutex = new SemaphoreSlim(1);
        
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

        static async Task OpenInternalAsync(ulong userId, uint amount)
        {
            var dbInventory = await DB.Inventory.GetAsync(userId);

            if (dbInventory.Chests == 0)
            {
                await RiftBot.SendMessageAsync("chests-nochests", Settings.ChannelId.Commands, new FormatData(userId));
                return;
            }

            if (dbInventory.Chests < amount || amount == 0)
            {
                await RiftBot.SendMessageAsync("chests-notenoughchests", Settings.ChannelId.Commands, new FormatData(userId));
                return;
            }

            await DB.Inventory.RemoveAsync(userId, new InventoryData {Chests = amount});
            ChestsOpened?.Invoke(null, new ChestsOpenedEventArgs(userId, amount));

            var chest = new ChestReward(amount);
            await chest.DeliverToAsync(userId);
            await DB.Statistics.AddAsync(userId, new StatisticData {ChestsOpened = amount});

            await RiftBot.SendMessageAsync("chests-open-success", Settings.ChannelId.Commands, new FormatData(userId)
            {
                Reward = chest
            });
        }
    }
}
