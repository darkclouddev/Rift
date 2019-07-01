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
        public async Task BuyItem(uint id = 0u)
        {
            using (Context.Channel.EnterTypingState())
            {
                var result = await storeService.PurchaseItemAsync(Context.User.Id, id);
                await Context.Channel.SendIonicMessageAsync(result);
            }
        }

        [Command("магазин ролей")]
        public async Task RoleShop()
        {
            await Context.Channel.SendIonicMessageAsync(storeService.RoleShopMessage);
        }

        [Command("купить роль")]
        [RequireContext(ContextType.Guild)]
        public async Task BuyRole(uint id = 0u)
        {
            using (Context.Channel.EnterTypingState())
            {
                var result = await storeService.PurchaseRoleAsync(Context.User.Id, id);
                await Context.Channel.SendIonicMessageAsync(result);
            }
        }

        [Command("магазин фонов")]
        public async Task BackgroundShop()
        {
            await Context.Channel.SendIonicMessageAsync(storeService.BackgroundShopMessage);
        }

        [Command("купить фон")]
        [RequireContext(ContextType.Guild)]
        public async Task BuyBack(uint id = 0u)
        {
            using (Context.Channel.EnterTypingState())
            {
                var result = await storeService.PurchaseBackgroundAsync(Context.User.Id, id);
                await Context.Channel.SendIonicMessageAsync(result);
            }
        }
    }
}
