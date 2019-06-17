using System.Threading.Tasks;

using Rift.Preconditions;
using Rift.Services;
using Rift.Services.Economy;
using Rift.Util;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using IonicLib.Util;

namespace Rift.Modules
{
    public class EconomyModule : RiftModuleBase
    {
        readonly EconomyService economyService;
        readonly RiotService riotService;

        public EconomyModule(EconomyService economyService, RiotService riotService)
        {
            this.economyService = economyService;
            this.riotService = riotService;
        }

        [Command("обновить")]
        [RateLimit(1, 10, Measure.Minutes, ErrorMessage = "Запрашивать обновление ранга можно не чаще 1 раза в 10 минут!")]
        [RequireContext(ContextType.Guild)]
        public async Task Update()
        {
            await riotService.UpdateRankAsync(Context.User.Id);
        }

        [Command("активные")]
        [RequireAdmin]
        [RequireContext(ContextType.Guild)]
        public async Task Active()
        {
            await EconomyService.ShowActiveUsersAsync();
        }

        [Command("богатые")]
        [RequireAdmin]
        [RequireContext(ContextType.Guild)]
        public async Task RichBitch()
        {
            await EconomyService.ShowRichUsersAsync();
        }

        [Command("профиль")]
        [RequireContext(ContextType.Guild)]
        public async Task Profile()
        {
            using (Context.Channel.EnterTypingState())
            {
                var message = await economyService.GetUserProfileAsync(Context.User.Id);

                if (message is null)
                    return;

                await Context.Channel.SendIonicMessageAsync(message);
            }
        }

        [Command("таймеры")]
        [Alias("кд")]
        [RateLimit(1, 10.0, Measure.Seconds, RateLimitFlags.NoLimitForAdmins, ErrorMessage = "Проверять таймеры можно не чаще 1 раза в 10 секунд!")]
        [RequireContext(ContextType.Guild)]
        public async Task Cooldowns()
        {
            using (Context.Channel.EnterTypingState())
            {
                var message = await economyService.GetUserCooldownsAsync(Context.User.Id);

                if (message is null)
                    return;

                await Context.Channel.SendIonicMessageAsync(message);
            }
        }

        [Command("двойной опыт")]
        [RequireContext(ContextType.Guild)]
        public async Task DoubleExp()
        {
            using (Context.Channel.EnterTypingState())
            {
                await economyService.ActivateDoubleExp(Context.User.Id);
            }
        }

        [Command("уважение ботов")]
        [RequireContext(ContextType.Guild)]
        public async Task BotRespect()
        {
            using (Context.Channel.EnterTypingState())
            {
                await economyService.ActivateBotRespect(Context.User.Id);
            }
        }

        [Command("игровой профиль")]
        [RateLimit(1, 10, Measure.Minutes, ErrorMessage = "Запрашивать игровой профиль можно не чаще 1 раза в 10 минут!")]
        [RequireContext(ContextType.Guild)]
        public async Task GameStat()
        {
            using (Context.Channel.EnterTypingState())
            {
                var message = await economyService.GetUserStatAsync(Context.User.Id);

                if (message is null)
                    return;

                await Context.Channel.SendIonicMessageAsync(message);
            }
        }

        [Command("статистика")]
        [RequireContext(ContextType.Guild)]
        public async Task Stat()
        {
            using (Context.Channel.EnterTypingState())
            {
                var message = await economyService.GetUserStatAsync(Context.User.Id);

                if (message is null)
                    return;

                await Context.Channel.SendIonicMessageAsync(message);
            }
        }

        [Command("похвастаться")]
        [RequireContext(ContextType.Guild)]
        public async Task Brag()
        {
            using (Context.Channel.EnterTypingState())
            {
                var message = await economyService.GetUserBragAsync(Context.User.Id);

                if (message is null)
                    return;

                await Context.Channel.SendIonicMessageAsync(message);
            }
        }

        [Group("открыть")]
        public class OpenModule : ModuleBase
        {
            readonly EconomyService economyService;
            
            public OpenModule(EconomyService economyService)
            {
                this.economyService = economyService;
            }

            [Command("сундук")]
            [RequireContext(ContextType.Guild)]
            public async Task Chest(uint amount = 1u)
            {
                using (Context.Channel.EnterTypingState())
                {
                    var message = await economyService.OpenChestAsync(Context.User.Id, amount);

                    if (message is null)
                        return;

                    await Context.Channel.SendIonicMessageAsync(message);
                }
            }

            [Command("все сундуки")]
            [RequireContext(ContextType.Guild)]
            public async Task AllChests()
            {
                using (Context.Channel.EnterTypingState())
                {
                    var message = await economyService.OpenChestAllAsync(Context.User.Id);

                    if (message is null)
                        return;

                    await Context.Channel.SendIonicMessageAsync(message);
                }
            }

            [Command("капсулу")]
            [RequireContext(ContextType.Guild)]
            public async Task Capsule()
            {
                var message = await economyService.OpenCapsuleAsync(Context.User.Id);

                await Context.Channel.SendIonicMessageAsync(message);
            }

            [Command("сферу")]
            [RequireContext(ContextType.Guild)]
            public async Task Sphere()
            {
                var embed = await economyService.OpenSphereAsync(Context.User.Id);

                await Context.Channel.SendIonicMessageAsync(embed);
            }
        }

        [Command("купить")]
        [RequireContext(ContextType.Guild)]
        public async Task Buy(uint id)
        {
            using (Context.Channel.EnterTypingState())
            {
                var storeItem = Store.GetShopItemById(id);

                if (storeItem is null)
                {
                    await Context.Channel.SendMessageAsync("Данный номер отсутствует в магазине.");
                    return;
                }

                var result = await economyService.StorePurchaseAsync(Context.User.Id, storeItem);
                await Context.Channel.SendIonicMessageAsync(result);
            }
        }

        [Command("магазин")]
        public async Task Shop()
        {
            await Context.User.SendEmbedAsync(Store.Embed);
        }
    }
}
