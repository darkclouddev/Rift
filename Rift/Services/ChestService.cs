using System;
using System.Threading;
using System.Threading.Tasks;

using Rift.Database;
using Rift.Events;
using Rift.Services.Message;
using Rift.Services.Reward;

namespace Rift.Services
{
    public class ChestService
    {
        public static event EventHandler<ChestsOpenedEventArgs> ChestsOpened;
        
        static readonly SemaphoreSlim Mutex = new SemaphoreSlim(1);
        
        public async Task<IonicMessage> OpenAsync(ulong userId, uint amount)
        {
            await Mutex.WaitAsync().ConfigureAwait(false);

            IonicMessage result;

            RiftBot.Log.Info($"Opening {amount.ToString()} chest(s) for {userId.ToString()}.");

            try
            {
                result = await OpenInternalAsync(userId, amount).ConfigureAwait(false);
            }
            finally
            {
                Mutex.Release();
            }

            return result;
        }

        public async Task<IonicMessage> OpenAllAsync(ulong userId)
        {
            await Mutex.WaitAsync().ConfigureAwait(false);

            IonicMessage result;

            try
            {
                var dbInventory = await DB.Inventory.GetAsync(userId);
                result = await OpenInternalAsync(userId, dbInventory.Chests).ConfigureAwait(false);
            }
            finally
            {
                Mutex.Release();
            }

            return result;
        }

        static async Task<IonicMessage> OpenInternalAsync(ulong userId, uint amount)
        {
            var dbInventory = await DB.Inventory.GetAsync(userId);

            if (dbInventory.Chests == 0)
                return await RiftBot.GetMessageAsync("chests-nochests", new FormatData(userId));
            
            if (dbInventory.Chests < amount || amount == 0)
                return await RiftBot.GetMessageAsync("chests-notenoughchests", new FormatData(userId));

            await DB.Inventory.RemoveAsync(userId, new InventoryData {Chests = amount});
            ChestsOpened?.Invoke(null, new ChestsOpenedEventArgs(userId, amount));

            var chest = new ChestReward(amount);
            await chest.DeliverToAsync(userId);
            await DB.Statistics.AddAsync(userId, new StatisticData {ChestsOpened = amount});

            return await RiftBot.GetMessageAsync("chests-open-success", new FormatData(userId)
            {
                Reward = chest
            });
        }
    }
}
