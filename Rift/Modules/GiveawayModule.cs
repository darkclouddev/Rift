using System.Threading.Tasks;

using Rift.Preconditions;
using Rift.Services;

using Discord.Commands;

namespace Rift.Modules
{
    public class GiveawayModule : RiftModuleBase
    {
        readonly GiveawayService giveawayService;

        public GiveawayModule(GiveawayService giveawayService)
        {
            this.giveawayService = giveawayService;
        }

        [Command("розыгрыш")]
        [RequireAdmin]
        [RequireContext(ContextType.Guild)]
        public async Task Start(int number)
        {
            await giveawayService.GiveawayStartAsync(number);
        }
    }
}
