using System.Threading.Tasks;

using Rift.Embeds;
using Rift.Preconditions;
using Rift.Services;

using IonicLib.Util;

using Discord;
using Discord.Commands;

namespace Rift.Modules
{
    public class RegisterModule : RiftModuleBase
    {
        readonly RiotService riotService;

        public RegisterModule(RiotService riotService)
        {
            this.riotService = riotService;
        }

        [Command("регистрация")]
        [RateLimit(2, 1, Measure.Hours)]
        [RequireContext(ContextType.Guild)]
        public async Task RegisterAsync(string region, [Remainder] string summonerName)
        {
            //var eb = new EmbedBuilder()
            //	.WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
            //	.WithColor(226, 87, 76)
            //	.WithDescription($"Функция регистрации аккаунтов в нашей системе временно отключена.");

            //await Context.User.SendEmbedAsync(eb);
            //return;

            var responseEmbed = await riotService.RegisterAsync(Context.User.Id, region, summonerName);

            await Context.User.SendEmbedAsync(responseEmbed);
        }

        [Command("отвязать")]
        [RequireModerator]
        [RequireContext(ContextType.Guild)]
        public async Task UnlinkAsync(IUser user)
        {
            if (user is null)
            {
                await Context.User.SendEmbedAsync(RegisterEmbeds.UserNotExists);
                return;
            }

            await Database.RemoveLolDataAsync(user.Id);
        }
    }
}
