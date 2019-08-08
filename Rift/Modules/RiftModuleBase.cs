using System.Threading.Tasks;

using Discord.Commands;

namespace Rift.Modules
{
    public class RiftModuleBase : ModuleBase
    {
        RiftModuleState moduleState;

        public RiftModuleBase()
        {
            moduleState = RiftModuleState.Enabled;
        }

        protected override void BeforeExecute(CommandInfo command)
        {
            if (moduleState == RiftModuleState.Disabled)
            {
                Task.Run(async () =>
                {
                    await base.ReplyAsync("В настоящий момент эта команда отключена администратором.");
                });
                return;
            }

            if (moduleState == RiftModuleState.AdminOnly && !RiftBot.IsAdmin(Context.User))
            {
                Task.Run(async () =>
                {
                    await base.ReplyAsync(
                        "В настоящий момент эта команда доступна только для администраторов.");
                });
                return;
            }
        }
    }

    public enum RiftModuleState
    {
        Disabled = 0,
        Enabled = 1,
        AdminOnly = 2,
    }
}
