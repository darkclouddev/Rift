using System;
using System.Threading.Tasks;

using Discord.Commands;

namespace Rift.Preconditions
{
    [AttributeUsage(AttributeTargets.Method)]
    class RequireAdminAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (await Task.Run(() => RiftBot.IsAdmin(context.User)))
                return PreconditionResult.FromSuccess();

            return PreconditionResult.FromError(RiftBot.CommandDenyMessage);
        }
    }
}
