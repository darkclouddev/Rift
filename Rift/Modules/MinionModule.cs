using System.Threading.Tasks;

using Rift.Preconditions;
using Rift.Services;

using Discord.Commands;

namespace Rift.Modules
{
    public class MinionModule : RiftModuleBase
    {
        readonly MinionService minionService;

        public MinionModule(MinionService minionService)
        {
            this.minionService = minionService;
        }

        [Command("миньон")]
        [RequireAdmin]
        [RequireContext(ContextType.Guild)]
        public async Task SpawnMinionAsync(uint minutes)
        {
            using (Context.Channel.EnterTypingState())
            {
                await minionService.SetupNextMinionAsync(minutes);
            }
        }

        [Command("убить миньона")]
        [RequireContext(ContextType.Guild)]
        public Task KillMinion()
        {
            using (Context.Channel.EnterTypingState())
            {
                minionService.SetUpKiller(Context.User.Id);
            }

            return Task.CompletedTask;
        }
    }
}
