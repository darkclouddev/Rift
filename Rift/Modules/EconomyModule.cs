using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using Rift.Configuration;
using Rift.Preconditions;
using Rift.Services.Interfaces;

namespace Rift.Modules
{
    public class EconomyModule : RiftModuleBase
    {
        readonly IEconomyService economyService;
        readonly IBragService bragService;
        readonly IGiveawayService giveawayService;
        readonly IBotRespectService botRespectService;
        readonly IBackgroundService backgroundService;
        readonly IRoleService roleService;
        readonly IRewindService rewindService;
        readonly IDoubleExpService doubleExpService;
        readonly IChannelService channelService;
        readonly IMessageService messageService;

        public EconomyModule(IEconomyService economyService,
                             IBragService bragService,
                             IGiveawayService giveawayService,
                             IBotRespectService botRespectService,
                             IBackgroundService backgroundService,
                             IRoleService roleService,
                             IRewindService rewindService,
                             IDoubleExpService doubleExpService,
                             IChannelService channelService,
                             IMessageService messageService)
        {
            this.economyService = economyService;
            this.bragService = bragService;
            this.giveawayService = giveawayService;
            this.botRespectService = botRespectService;
            this.backgroundService = backgroundService;
            this.roleService = roleService;
            this.rewindService = rewindService;
            this.doubleExpService = doubleExpService;
            this.channelService = channelService;
            this.messageService = messageService;
        }
        
        [Command("задания")]
        [Alias("квесты")]
        [RequireContext(ContextType.Guild)]
        public async Task Quests()
        {
            await economyService.GetQuests(Context.User.Id);
        }

        [Command("выгнать")]
        [RequireContext(ContextType.Guild)]
        public async Task BanFromChannel(IUser targetUser)
        {
            await channelService.DenyAccessToUserAsync(Context.User, targetUser);
        }

        [Command("поставить роль")]
        [RequireContext(ContextType.Guild)]
        public async Task AddRole(int id)
        {
            await roleService.UpdateInventoryRoleAsync(Context.User.Id, id, true);
        }

        [Command("убрать роль")]
        [RequireContext(ContextType.Guild)]
        public async Task RemoveRole(int id)
        {
            await roleService.UpdateInventoryRoleAsync(Context.User.Id, id, false);
        }

        [Command("поставить фон")]
        [RequireContext(ContextType.Guild)]
        public async Task SetBackground(int id)
        {
            await backgroundService.SetActiveAsync(Context.User.Id, id);
        }

        [Command("роли")]
        [RequireContext(ContextType.Guild)]
        public async Task RoleInventoryList()
        {
            await roleService.GetInventoryAsync(Context.User.Id);
        }

        [Command("фоны")]
        [RequireContext(ContextType.Guild)]
        public async Task BackgroundInventoryList()
        {
            await backgroundService.GetInventoryAsync(Context.User.Id);
        }

        [Command("выдать билеты")]
        [RequireTicketKeeper]
        [RequireContext(ContextType.Guild)]
        public async Task GiveTickets()
        {
            await giveawayService.GiveTicketsToLowLevelUsersAsync(Context.User.Id);
        }

        [Command("активные")]
        [RequireContext(ContextType.Guild)]
        public async Task Active()
        {
            await economyService.ShowActiveUsersAsync();
        }

        [Command("богатые")]
        [RequireContext(ContextType.Guild)]
        public async Task RichBitch()
        {
            await economyService.ShowRichUsersAsync();
        }

        [Command("профиль")]
        [RequireContext(ContextType.Guild)]
        public async Task Profile(IUser user = null)
        {
            using (Context.Channel.EnterTypingState())
            {
                await economyService.GetUserProfileAsync(user?.Id ?? Context.User.Id);
            }
        }
        
        [Command("таймеры")]
        [Alias("кд")]
        [RateLimit(1, 10.0, Measure.Seconds, RateLimitFlags.NoLimitForAdmins,
            ErrorMessage = "Проверять таймеры можно не чаще 1 раза в 10 секунд!")]
        [RequireContext(ContextType.Guild)]
        public async Task Cooldowns()
        {
            using (Context.Channel.EnterTypingState())
            {
                await economyService.GetUserCooldownsAsync(Context.User.Id);
            }
        }

        [Command("двойной опыт")]
        [RequireContext(ContextType.Guild)]
        public async Task DoubleExp()
        {
            using (Context.Channel.EnterTypingState())
            {
                await doubleExpService.ActivateAsync(Context.User.Id);
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

        [Command("перемотка")]
        [RequireContext(ContextType.Guild)]
        public async Task Rewind()
        {
            using (Context.Channel.EnterTypingState())
            {
                await rewindService.ActivateAsync(Context.User.Id);
            }
        }

        [Command("статистика")]
        [RequireContext(ContextType.Guild)]
        public async Task Stat()
        {
            using (Context.Channel.EnterTypingState())
            {
                await economyService.GetUserStatAsync(Context.User.Id);
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
        
        [Command("роли")]
        [RequireContext(ContextType.Guild)]
        public async Task Roles()
        {
            using (Context.Channel.EnterTypingState())
            {
                await messageService.SendMessageAsync("roles-list", Settings.ChannelId.Commands, null);
            }
        }

        [Group("открыть")]
        public class OpenModule : ModuleBase
        {
            readonly IChestService chestService;
            readonly ICapsuleService capsuleService;
            readonly ISphereService sphereService;

            public OpenModule(IChestService chestService,
                              ICapsuleService capsuleService,
                              ISphereService sphereService)
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
                    await chestService.OpenAsync(Context.User.Id, amount);
                }
            }

            [Command("все сундуки")]
            [RequireContext(ContextType.Guild)]
            public async Task AllChests()
            {
                using (Context.Channel.EnterTypingState())
                {
                    await chestService.OpenAllAsync(Context.User.Id);
                }
            }

            [Command("капсулу")]
            [RequireContext(ContextType.Guild)]
            public async Task Capsule()
            {
                using (Context.Channel.EnterTypingState())
                {
                    await capsuleService.OpenAsync(Context.User.Id);
                }
            }

            [Command("сферу")]
            [RequireContext(ContextType.Guild)]
            public async Task Sphere()
            {
                await sphereService.OpenAsync(Context.User.Id);
            }
        }
    }
}
