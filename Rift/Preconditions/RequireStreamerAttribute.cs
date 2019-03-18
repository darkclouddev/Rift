using System;
using System.Threading.Tasks;

using Discord.Commands;

namespace Rift.Preconditions
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequireStreamerAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context,
            CommandInfo command, IServiceProvider services)
        {
            return await Database.GetStreamer(context.User.Id) is null
                ? PreconditionResult.FromError(RiftBot.CommandDenyMessage)
                : PreconditionResult.FromSuccess();
        }
    }
}
