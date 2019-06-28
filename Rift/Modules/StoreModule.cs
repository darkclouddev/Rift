using System.Threading.Tasks;

using Rift.Services;
using Rift.Util;

using Discord.Commands;

namespace Rift.Modules
{
    public class StoreModule : RiftModuleBase
    {
        readonly StoreService storeService;

        public StoreModule(StoreService storeService)
        {
            this.storeService = storeService;
        }

        [Command("магазин")]
        public async Task ItemShop()
        {
            await Context.Channel.SendIonicMessageAsync(storeService.ItemShopMessage);
        }

        [Command("купить")]
        [RequireContext(ContextType.Guild)]
        public async Task Buy(uint id = 0u)
        {
            using (Context.Channel.EnterTypingState())
            {
                var result = await storeService.PurchaseItemAsync(Context.User.Id, id);
                await Context.Channel.SendIonicMessageAsync(result);
            }
        }
    }
}
