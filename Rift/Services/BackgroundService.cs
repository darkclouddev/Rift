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
        
        
    }
}
