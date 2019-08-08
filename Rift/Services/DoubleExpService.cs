using System;
using System.Threading;
using System.Threading.Tasks;

using Rift.Database;
using Rift.Services.Message;

using Settings = Rift.Configuration.Settings;

namespace Rift.Services
{
    public class DoubleExpService
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
                RiftBot.Log.Error("An error occured when activating double exp:");
                RiftBot.Log.Error(ex);
            }
            finally
            {
                Mutex.Release();
            }
        }
        
        async Task ActivateInternalAsync(ulong userId)
        {
            var channel = Settings.ChannelId.Commands;
            
            var dbInventory = await DB.Inventory.GetAsync(userId);

            if (dbInventory.BonusDoubleExp == 0)
            {
                await RiftBot.SendMessageAsync("bonus-nobonus", channel, new FormatData(userId));
                return;
            }

            var dbDoubleExp = await DB.Cooldowns.GetAsync(userId);
            if (dbDoubleExp.DoubleExpTime > DateTime.UtcNow)
            {
                await RiftBot.SendMessageAsync("bonus-active", channel, new FormatData(userId));
                return;
            }

            await DB.Inventory.RemoveAsync(userId, new InventoryData {DoubleExps = 1});

            var dateTime = DateTime.UtcNow.AddHours(12.0);
            await DB.Cooldowns.SetDoubleExpTimeAsync(userId, dateTime);

            await RiftBot.SendMessageAsync("doubleexp-success", channel, new FormatData(userId));
        }
    }
}
