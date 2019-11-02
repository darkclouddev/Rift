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
    public class SphereService : ISphereService
    {
        public event EventHandler<OpenedSphereEventArgs> OpenedSphere;
        
        static readonly SemaphoreSlim Mutex = new SemaphoreSlim(1);

        readonly IMessageService messageService;
        readonly IRewardService rewardService;

        public SphereService(IMessageService messageService,
                             IRewardService rewardService)
        {
            this.messageService = messageService;
            this.rewardService = rewardService;
        }
        
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

        async Task OpenInternalAsync(ulong userId)
        {
            var dbInventory = await DB.Inventory.GetAsync(userId);

            if (dbInventory.Spheres == 0u)
            {
                await messageService.SendMessageAsync("spheres-nospheres", Settings.ChannelId.Commands, new FormatData(userId));
                return;
            }

            await DB.Inventory.RemoveAsync(userId, new InventoryData {Spheres = 1u});
            OpenedSphere?.Invoke(null, new OpenedSphereEventArgs(userId));

            var sphere = new SphereReward();
            await rewardService.DeliverToAsync(userId, sphere);
            await DB.Statistics.AddAsync(userId, new StatisticData {SpheresOpened = 1u});

            await messageService.SendMessageAsync("spheres-open-success", Settings.ChannelId.Commands, new FormatData(userId)
            {
                Reward = sphere
            });
        }
    }
}
