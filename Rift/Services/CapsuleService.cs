using System.Threading;
using System.Threading.Tasks;

using Rift.Database;
using Rift.Services.Message;
using Rift.Services.Reward;

namespace Rift.Services
{
    public class CapsuleService
    {
        static readonly SemaphoreSlim Mutex = new SemaphoreSlim(1);
        
        public async Task<IonicMessage> OpenAsync(ulong userId)
        {
            await Mutex.WaitAsync().ConfigureAwait(false);

            IonicMessage result;

            RiftBot.Log.Info($"Opening capsule for {userId.ToString()}.");

            try
            {
                result = await OpenInternalAsync(userId).ConfigureAwait(false);
            }
            finally
            {
                Mutex.Release();
            }

            return result;
        }

        static async Task<IonicMessage> OpenInternalAsync(ulong userId)
        {
            var dbUserInventory = await DB.Inventory.GetAsync(userId);

            if (dbUserInventory.Capsules == 0u)
                await RiftBot.GetMessageAsync("capsules-nocapsules", new FormatData(userId));

            await DB.Inventory.RemoveAsync(userId, new InventoryData {Capsules = 1u});

            var capsule = new CapsuleReward();
            await capsule.DeliverToAsync(userId);
            await DB.Statistics.AddAsync(userId, new StatisticData {CapsulesOpened = 1u});

            return await RiftBot.GetMessageAsync("capsules-open-success", new FormatData(userId)
            {
                Reward = capsule
            });
        }
    }
}
