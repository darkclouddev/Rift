using System.Threading.Tasks;

using Rift.Preconditions;
using Rift.Services;
using Rift.Services.Message;
using Rift.Util;

using Discord;
using Discord.Commands;

using Rift.Configuration;

namespace Rift.Modules
{
    public class LeagueModule : RiftModuleBase
    {
        readonly RiotService riotService;

        public LeagueModule(RiotService riotService)
        {
            this.riotService = riotService;
        }

        [Command("регистрация")]
        [RequireContext(ContextType.Guild)]
        public async Task RegisterAsync(string region, [Remainder] string summonerName)
        {
            var message = await riotService.RegisterAsync(Context.User.Id, region, summonerName);

            await Context.Channel.SendIonicMessageAsync(message);
        }
        
        [Command("обновить")]
        [RateLimit(1, 10, Measure.Minutes,
            ErrorMessage = "Запрашивать обновление ранга можно не чаще 1 раза в 10 минут!")]
        [RequireContext(ContextType.Guild)]
        public async Task Update()
        {
            await riotService.UpdateSummonerAsync(Context.User.Id);
        }
        
        [Command("игровой профиль")]
        [RateLimit(1, 10, Measure.Minutes, ErrorMessage =
            "Запрашивать игровой профиль можно не чаще 1 раза в 10 минут!")]
        [RequireContext(ContextType.Guild)]
        public async Task GameStat()
        {
            using (Context.Channel.EnterTypingState())
            {
                var message = await riotService.GetUserGameStatAsync(Context.User.Id);
                await Context.Channel.SendIonicMessageAsync(message);
            }
        }

        [Command("отвязать")]
        [RequireModerator]
        [RequireContext(ContextType.Guild)]
        public async Task UnlinkAsync(IUser user)
        {
            if (user is null)
            {
                await Context.Channel.SendIonicMessageAsync(MessageService.UserNotFound);
                return;
            }

            await DB.LeagueData.RemoveAsync(user.Id);
            await RiftBot.SendMessageAsync("loldata-removed", Settings.ChannelId.Commands, new FormatData(user.Id));
        }
    }
}
