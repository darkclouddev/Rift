using System;
using System.Net;
using System.Threading.Tasks;

using Discord;

using Rift.Services.Interfaces;
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
        readonly IEconomyService economyService;

        public TestModule(IEconomyService economyService)
        {
            this.economyService = economyService;
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
            await ReplyAsync($"Lv {level.ToString()}: {economyService.GetExpForLevel(level).ToString()} XP");
        }
    }
}
