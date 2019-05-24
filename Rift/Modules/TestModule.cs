using System.Net;
using System.Threading.Tasks;

using Rift.Services;
using Rift.Preconditions;

using Discord.Commands;
using Discord.WebSocket;
using Humanizer;

namespace Rift.Modules
{
    [Group("test")]
    [Alias("тест")]
    public class TestModule : RiftModuleBase
    {
        readonly EconomyService economyService;
        readonly RoleService roleService;
        readonly MessageService messageService;
        readonly EventService eventService;

        public TestModule(/*EconomyService economyService, RoleService roleService, MessageService messageService,
            EventService eventService*/)
        {
            this.economyService = economyService;
            this.roleService = roleService;
            this.messageService = messageService;
            this.eventService = eventService;
        }

        [Command("since")]
        [RateLimit(1, 10, Measure.Minutes, RateLimitFlags.NoLimitForAdmins)]
        [RequireContext(ContextType.Guild)]
        public async Task Since()
        {
            await ReplyAsync($"{Context.User.Mention} на сервере с"
                             + $" {((SocketGuildUser) Context.User).JoinedAt.Value.LocalDateTime.Humanize()}"); // TODO: do proper check
        }

        [Command("baron")]
        [RequireDeveloper]
        public async Task Baron()
        {
            await eventService.StartEvent(EventType.Baron);
        }

        [Command("horse")]
        [RequireDeveloper]
        public async Task Horse()
        {
            var url = "http://www.merlinsltd.com/WebRoot/StoreLGB/Shops/62030553/54C2/CD1F/F497/BF84/54D1/C0A8/2ABB/1A80/mask_horse_brown.png";

            var request = WebRequest.Create(url);

            using (var stream = (await request.GetResponseAsync()).GetResponseStream())
            {
                await Context.Channel.SendFileAsync(stream, "horse.png");
            }
        }

        [Command("exp")]
        [RequireAdmin]
        [RequireContext(ContextType.DM)]
        public async Task Exp(uint level)
        {
            await ReplyAsync($"Lv {level.ToString()}: {EconomyService.GetExpForLevel(level).ToString()} XP");
        }
    }
}
