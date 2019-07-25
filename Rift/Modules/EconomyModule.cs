using System.Threading.Tasks;

using Rift.Preconditions;
using Rift.Services;
using Rift.Util;

using Discord.Commands;

namespace Rift.Modules
{
    public class EconomyModule : RiftModuleBase
    {
        readonly EconomyService economyService;
        readonly RiotService riotService;
        readonly BragService bragService;
        readonly GiveawayService giveawayService;
        readonly BotRespectService botRespectService;
        readonly BackgroundService backgroundService;
        readonly RoleService roleService;

        public EconomyModule(EconomyService economyService, RiotService riotService, BragService bragService,
                             GiveawayService giveawayService, BotRespectService botRespectService,
                             BackgroundService backgroundService, RoleService roleService)
        {
            this.economyService = economyService;
            this.riotService = riotService;
            this.bragService = bragService;
            this.giveawayService = giveawayService;
            this.botRespectService = botRespectService;
            this.backgroundService = backgroundService;
            this.roleService = roleService;
        }

        [Command("поставить роль")]
        [RequireDeveloper]
        [RequireContext(ContextType.Guild)]
        public async Task AddRole(int id)
        {
            await roleService.UpdateInventoryRoleAsync(Context.User.Id, id, true);
        }

        [Command("убрать роль")]
        [RequireDeveloper]
        [RequireContext(ContextType.Guild)]
        public async Task RemoveRole(int id)
        {
            await roleService.UpdateInventoryRoleAsync(Context.User.Id, id, false);
        }

        [Command("поставить фон")]
        [RequireDeveloper]
        [RequireContext(ContextType.Guild)]
        public async Task SetBackground(int id)
        {
            await backgroundService.SetActiveAsync(Context.User.Id, id);
        }

        [Command("роли")]
        [RequireDeveloper]
        [RequireContext(ContextType.Guild)]
        public async Task RoleInventoryList()
        {
            await roleService.GetInventoryAsync(Context.User.Id);
        }

        [Command("фоны")]
        [RequireDeveloper]
        [RequireContext(ContextType.Guild)]
        public async Task BackgroundInventoryList()
        {
            await backgroundService.GetInventoryAsync(Context.User.Id);
        }

        [Command("выдать билеты")]
        [RequireDeveloper]
        [RequireContext(ContextType.Guild)]
        public async Task GiveTickets()
        {
            await giveawayService.GiveTicketsToLowLevelUsersAsync(Context.User.Id);
        }

        [Command("обновить")]
        [RateLimit(1, 10, Measure.Minutes, ErrorMessage =
            "Запрашивать обновление ранга можно не чаще 1 раза в 10 минут!")]
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
                await Context.Channel.SendIonicMessageAsync(message).ConfigureAwait(false);
            }
        }

        [Command("таймеры")]
        [Alias("кд")]
        [RateLimit(1, 10.0, Measure.Seconds, RateLimitFlags.NoLimitForAdmins, ErrorMessage =
            "Проверять таймеры можно не чаще 1 раза в 10 секунд!")]
        [RequireContext(ContextType.Guild)]
        public async Task Cooldowns()
        {
            using (Context.Channel.EnterTypingState())
            {
                var message = await economyService.GetUserCooldownsAsync(Context.User.Id);
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
                await botRespectService.ActivateAsync(Context.User.Id);
            }
        }

        [Command("игровой профиль")]
        [RateLimit(1, 10, Measure.Minutes, ErrorMessage =
            "Запрашивать игровой профиль можно не чаще 1 раза в 10 минут!")]
        [RequireContext(ContextType.Guild)]
        public async Task GameStat()
        {
            using (Context.Channel.EnterTypingState())
            {
                var message = await economyService.GetUserStatAsync(Context.User.Id);
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
                await Context.Channel.SendIonicMessageAsync(message);
            }
        }

        [Command("похвастаться")]
        [RequireContext(ContextType.Guild)]
        public async Task Brag()
        {
            using (Context.Channel.EnterTypingState())
            {
                await bragService.GetUserBragAsync(Context.User.Id);
            }
        }

        [Group("открыть")]
        public class OpenModule : ModuleBase
        {
            readonly ChestService chestService;
            readonly CapsuleService capsuleService;
            readonly SphereService sphereService;

            public OpenModule(ChestService chestService, CapsuleService capsuleService, SphereService sphereService)
            {
                this.chestService = chestService;
                this.capsuleService = capsuleService;
                this.sphereService = sphereService;
            }

            [Command("сундук")]
            [RequireContext(ContextType.Guild)]
            public async Task Chest(uint amount = 1u)
            {
                using (Context.Channel.EnterTypingState())
                {
                    var message = await chestService.OpenAsync(Context.User.Id, amount);
                    await Context.Channel.SendIonicMessageAsync(message);
                }
            }

            [Command("все сундуки")]
            [RequireContext(ContextType.Guild)]
            public async Task AllChests()
            {
                using (Context.Channel.EnterTypingState())
                {
                    var message = await chestService.OpenAllAsync(Context.User.Id);
                    await Context.Channel.SendIonicMessageAsync(message);
                }
            }

            [Command("капсулу")]
            [RequireContext(ContextType.Guild)]
            public async Task Capsule()
            {
                var message = await capsuleService.OpenAsync(Context.User.Id);
                await Context.Channel.SendIonicMessageAsync(message);
            }

            [Command("сферу")]
            [RequireContext(ContextType.Guild)]
            public async Task Sphere()
            {
                var message = await sphereService.OpenAsync(Context.User.Id);
                await Context.Channel.SendIonicMessageAsync(message);
            }
        }
    }
}
