using System;
using System.Threading;
using System.Threading.Tasks;

using Rift.Database;
using Rift.Services.Message;

using Settings = Rift.Configuration.Settings;

namespace Rift.Services
{
    public class RewindService
    {
        static readonly SemaphoreSlim Mutex = new SemaphoreSlim(1);
        
        public async Task ActivateAsync(ulong userId)
        {
            await Mutex.WaitAsync();

            try
            {
                await ActivateInternalAsync(userId);
            }
            catch (Exception ex)
            {
                RiftBot.Log.Error("An error occured while activating rewind:");
                RiftBot.Log.Error(ex);
            }
            finally
            {
                Mutex.Release();
            }
        }

        async Task ActivateInternalAsync(ulong userId)
        {
            var dbInv = await DB.Inventory.GetAsync(userId);

            if (dbInv.BonusRewind == 0u)
            {
                await RiftBot.SendMessageAsync("bonus-nobonus", Settings.ChannelId.Commands, new FormatData(userId));
                return;
            }

            await DB.Inventory.RemoveAsync(userId, new InventoryData {Rewinds = 1u});
            await DB.Cooldowns.ResetAsync(userId);
            await DB.Statistics.AddAsync(userId, new StatisticData {RewindsActivated = 1u});
            
            await RiftBot.SendMessageAsync("rewind-success", Settings.ChannelId.Commands, new FormatData(userId));
        }
    }
}
