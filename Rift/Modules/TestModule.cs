using System;
using System.Net;
using System.Threading.Tasks;

using Discord;

using Rift.Services;
using Rift.Preconditions;

using Discord.Commands;
using Discord.WebSocket;

using Humanizer;
using Humanizer.Localisation;

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

        public TestModule( /*EconomyService economyService, RoleService roleService, MessageService messageService,
            EventService eventService*/)
        {
            economyService = economyService;
            roleService = roleService;
            messageService = messageService;
            eventService = eventService;
        }

        [Command("since")]
        [RequireAdmin]
        [RateLimit(1, 10, Measure.Minutes, RateLimitFlags.NoLimitForAdmins)]
        [RequireContext(ContextType.Guild)]
        public async Task Since(IUser user)
        {
            if (!(user is SocketGuildUser sgUser))
                return;

            await ReplyAsync(
                $"{user.Mention} на сервере {(DateTime.UtcNow - sgUser.JoinedAt.Value.UtcDateTime).Humanize(3, minUnit: TimeUnit.Day, maxUnit: TimeUnit.Year)}"); // TODO: do proper check
        }

        [Command("horse")]
        [RequireDeveloper]
        public async Task Horse()
        {
            var url = "https://cdn.ionpri.me/rift/things/horse_mask.png";

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
