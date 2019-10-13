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
    public class SphereService
    {
        public static event EventHandler<OpenedSphereEventArgs> OpenedSphere;
        
        static readonly SemaphoreSlim Mutex = new SemaphoreSlim(1);
        
        public async Task OpenAsync(ulong userId)
        {
            await Mutex.WaitAsync().ConfigureAwait(false);

            RiftBot.Log.Information($"Opening sphere for {userId.ToString()}.");

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
            var dbInventory = await DB.Inventory.GetAsync(userId);

            if (dbInventory.Spheres == 0u)
            {
                await RiftBot.SendMessageAsync("spheres-nospheres", Settings.ChannelId.Commands, new FormatData(userId));
                return;
            }

            await DB.Inventory.RemoveAsync(userId, new InventoryData {Spheres = 1u});
            OpenedSphere?.Invoke(null, new OpenedSphereEventArgs(userId));

            var sphere = new SphereReward();
            await sphere.DeliverToAsync(userId);
            await DB.Statistics.AddAsync(userId, new StatisticData {SpheresOpened = 1u});

            await RiftBot.SendMessageAsync("spheres-open-success", Settings.ChannelId.Commands, new FormatData(userId)
            {
                Reward = sphere
            });
        }
    }
}
