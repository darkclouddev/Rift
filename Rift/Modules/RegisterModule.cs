using System.Threading.Tasks;

using Rift.Preconditions;
using Rift.Services;
using Rift.Services.Message;
using Rift.Util;

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
        [RequireContext(ContextType.Guild)]
        public async Task RegisterAsync(string region, [Remainder] string summonerName)
        {
            var message = await riotService.RegisterAsync(Context.User.Id, region, summonerName);
            
            await Context.Channel.SendIonicMessageAsync(message);
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

            await DB.LolData.RemoveAsync(user.Id);
            var msg = await RiftBot.GetMessageAsync("loldata-removed", new FormatData(user.Id));
            await Context.Channel.SendIonicMessageAsync(msg);
        }
    }
}
