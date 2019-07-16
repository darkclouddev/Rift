using System.Threading;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Services.Message;

namespace Rift.Services
{
    public class BackgroundService
    {
        static readonly SemaphoreSlim Mutex = new SemaphoreSlim(1);

        const string InventoryIdentifier = "background-inventory-list";

        public async Task GetInventoryAsync(ulong userId)
        {
            await RiftBot.SendMessageAsync(InventoryIdentifier, Settings.ChannelId.Comms, new FormatData(userId))
                         .ConfigureAwait(false);
        }

        public async Task SetActiveAsync(ulong userId, int backgroundId)
        {
            var setDefault = backgroundId == 0;
            
            if (!setDefault && !await DB.BackgroundInventory.HasAsync(userId, backgroundId))
            {
                await RiftBot.SendMessageAsync("backgrounds-wrongnumber", Settings.ChannelId.Comms, new FormatData(userId));
                return;
            }

            var dbUser = await DB.Users.GetAsync(userId);
            if (dbUser is null)
            {
                await RiftBot.SendMessageAsync(MessageService.UserNotFound, Settings.ChannelId.Comms);
                return;
            }
            
            if (!setDefault && dbUser.ProfileBackground == backgroundId)
            {
                await RiftBot.SendMessageAsync("backgrounds-alreadyactive", Settings.ChannelId.Comms, new FormatData(userId));
                return;
            }

            await DB.Users.SetBackgroundAsync(userId, backgroundId);
            await RiftBot.SendMessageAsync("backgrounds-set-success", Settings.ChannelId.Comms, new FormatData(userId));
        }
    }
}
