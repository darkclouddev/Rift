using System;
using System.Threading;
using System.Threading.Tasks;

using Rift.Database;
using Rift.Services.Interfaces;
using Rift.Services.Message;

using Settings = Rift.Configuration.Settings;

namespace Rift.Services
{
    public class RewindService : IRewindService
    {
        static readonly SemaphoreSlim Mutex = new SemaphoreSlim(1);

        readonly IMessageService messageService;

        public RewindService(IMessageService messageService)
        {
            this.messageService = messageService;
        }
        
        public async Task ActivateAsync(ulong userId)
        {
            await Mutex.WaitAsync();

            try
            {
                await ActivateInternalAsync(userId);
            }
            catch (Exception ex)
            {
                RiftBot.Log.Error(ex, "An error occured while activating rewind:");
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
                await messageService.SendMessageAsync("bonus-nobonus", Settings.ChannelId.Commands, new FormatData(userId));
                return;
            }

            await DB.Inventory.RemoveAsync(userId, new InventoryData {Rewinds = 1u});
            await DB.Cooldowns.ResetAsync(userId);
            await DB.Statistics.AddAsync(userId, new StatisticData {RewindsActivated = 1u});
            
            await messageService.SendMessageAsync("rewind-success", Settings.ChannelId.Commands, new FormatData(userId));
        }
    }
}
