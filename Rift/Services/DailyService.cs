using System;
using System.Threading;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Services.Interfaces;
using Rift.Services.Message;

namespace Rift.Services
{
    public class DailyService : IDailyService
    {
        static readonly SemaphoreSlim Mutex = new SemaphoreSlim(1);
        const int RewardId = 23;

        readonly IMessageService messageService;
        readonly IRewardService rewardService;

        public DailyService(IMessageService messageService,
                            IRewardService rewardService)
        {
            this.messageService = messageService;
            this.rewardService = rewardService;
        }
        
        public async Task CheckAsync(ulong userId)
        {
            await Mutex.WaitAsync();
            
            try
            {
                await CheckInternalAsync(userId);
            }
            catch (Exception ex)
            {
                RiftBot.Log.Error(ex, "An error occured while checking daily reward:");
            }
            finally
            {
                Mutex.Release();
            }
        }
        
        async Task CheckInternalAsync(ulong userId)
        {
            var cds = await DB.Cooldowns.GetAsync(userId);

            if (cds.DailyRewardTimeSpan != TimeSpan.Zero)
                return;

            await GiveRewardAsync(userId);
        }

        async Task GiveRewardAsync(ulong userId)
        {
            var dbReward = await DB.Rewards.GetAsync(RewardId);

            if (dbReward is null)
            {
                RiftBot.Log.Error($"Reward not found: ID {RewardId.ToString()}");
                return;
            }

            var reward = dbReward.ToRewardBase();

            await rewardService.DeliverToAsync(userId, reward);

            await DB.Cooldowns.SetLastDailyRewardTimeAsync(userId, DateTime.UtcNow);
            
            await messageService.SendMessageAsync("daily-success", Settings.ChannelId.Commands, new FormatData(userId)
            {
                Reward = reward
            });
        }
    }
}
