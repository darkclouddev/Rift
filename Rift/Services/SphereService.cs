using System;
using System.Threading;
using System.Threading.Tasks;

using Rift.Database;
using Rift.Events;
using Rift.Services.Message;
using Rift.Services.Reward;

namespace Rift.Services
{
    public class SphereService
    {
        public static event EventHandler<OpenedSphereEventArgs> OpenedSphere;
        
        static readonly SemaphoreSlim Mutex = new SemaphoreSlim(1);
        
        public async Task<IonicMessage> OpenAsync(ulong userId)
        {
            await Mutex.WaitAsync().ConfigureAwait(false);

            IonicMessage result;

            RiftBot.Log.Info($"Opening sphere for {userId.ToString()}.");

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
            var dbInventory = await DB.Inventory.GetAsync(userId);

            if (dbInventory.Spheres == 0u)
                return await RiftBot.GetMessageAsync("spheres-nospheres", new FormatData(userId));

            await DB.Inventory.RemoveAsync(userId, new InventoryData {Spheres = 1u});
            OpenedSphere?.Invoke(null, new OpenedSphereEventArgs(userId));

            var sphere = new SphereReward();
            await sphere.DeliverToAsync(userId);
            await DB.Statistics.AddAsync(userId, new StatisticData {SpheresOpened = 1u});

            return await RiftBot.GetMessageAsync("spheres-open-success", new FormatData(userId));
        }
    }
}
