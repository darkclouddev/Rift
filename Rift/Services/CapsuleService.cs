using System.Threading;
using System.Threading.Tasks;

using Rift.Database;
using Rift.Services.Interfaces;
using Rift.Services.Message;
using Rift.Services.Reward;

using Settings = Rift.Configuration.Settings;

namespace Rift.Services
{
    public class CapsuleService : ICapsuleService
    {
        static readonly SemaphoreSlim Mutex = new SemaphoreSlim(1);

        readonly IMessageService messageService;
        readonly IRewardService rewardService;

        public CapsuleService(IMessageService messageService,
                              IRewardService rewardService)
        {
            this.messageService = messageService;
            this.rewardService = rewardService;
        }
        
        public async Task OpenAsync(ulong userId)
        {
            await Mutex.WaitAsync().ConfigureAwait(false);

            RiftBot.Log.Information($"Opening capsule for {userId.ToString()}.");

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
            var dbUserInventory = await DB.Inventory.GetAsync(userId);

            if (dbUserInventory.Capsules == 0u)
            {
                await messageService.SendMessageAsync("capsules-nocapsules", Settings.ChannelId.Commands, new FormatData(userId));
                return;
            }

            await DB.Inventory.RemoveAsync(userId, new InventoryData {Capsules = 1u});

            var reward = new CapsuleReward();
            await rewardService.DeliverToAsync(userId, reward);
            await DB.Statistics.AddAsync(userId, new StatisticData {CapsulesOpened = 1u});

            await messageService.SendMessageAsync("capsules-open-success", Settings.ChannelId.Commands, new FormatData(userId)
            {
                Reward = reward
            });
        }
    }
}
