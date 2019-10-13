using System;
using System.Threading;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Services.Message;

namespace Rift.Services
{
    public class DailyService
    {
        static readonly SemaphoreSlim Mutex = new SemaphoreSlim(1);
        const int RewardId = 23;
        
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
        
        static async Task CheckInternalAsync(ulong userId)
        {
            var cds = await DB.Cooldowns.GetAsync(userId);

            if (cds.DailyRewardTimeSpan != TimeSpan.Zero)
                return;

            await GiveRewardAsync(userId);
        }

        static async Task GiveRewardAsync(ulong userId)
        {
            var dbReward = await DB.Rewards.GetAsync(RewardId);

            if (dbReward is null)
            {
                RiftBot.Log.Error($"Reward not found: ID {RewardId.ToString()}");
                return;
            }

            var reward = dbReward.ToRewardBase();

            await reward.DeliverToAsync(userId);

            await DB.Cooldowns.SetLastDailyRewardTimeAsync(userId, DateTime.UtcNow);
            
            await RiftBot.SendMessageAsync("daily-success", Settings.ChannelId.Commands, new FormatData(userId)
            {
                Reward = reward
            });
        }
    }
}
