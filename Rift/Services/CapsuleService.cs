using System.Threading;
using System.Threading.Tasks;

using Rift.Database;
using Rift.Services.Message;
using Rift.Services.Reward;

using Settings = Rift.Configuration.Settings;

namespace Rift.Services
{
    public class CapsuleService
    {
        static readonly SemaphoreSlim Mutex = new SemaphoreSlim(1);
        
        public async Task OpenAsync(ulong userId)
        {
            await Mutex.WaitAsync().ConfigureAwait(false);

            RiftBot.Log.Info($"Opening capsule for {userId.ToString()}.");

            try
            {
                await OpenInternalAsync(userId).ConfigureAwait(false);
            }
            finally
            {
                Mutex.Release();
            }
        }

        static async Task OpenInternalAsync(ulong userId)
        {
            var dbUserInventory = await DB.Inventory.GetAsync(userId);

            if (dbUserInventory.Capsules == 0u)
            {
                await RiftBot.SendMessageAsync("capsules-nocapsules", Settings.ChannelId.Commands, new FormatData(userId));
                return;
            }

            await DB.Inventory.RemoveAsync(userId, new InventoryData {Capsules = 1u});

            var capsule = new CapsuleReward();
            await capsule.DeliverToAsync(userId);
            await DB.Statistics.AddAsync(userId, new StatisticData {CapsulesOpened = 1u});

            await RiftBot.SendMessageAsync("capsules-open-success", Settings.ChannelId.Commands, new FormatData(userId)
            {
                Reward = capsule
            });
        }
    }
}
