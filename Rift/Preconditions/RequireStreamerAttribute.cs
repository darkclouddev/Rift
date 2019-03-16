using System;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Discord.Commands;

using Rift.Services;

namespace Rift.Preconditions
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequireStreamerAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            return await services.GetService<DatabaseService>().GetStreamer(context.User.Id) is null
                ? PreconditionResult.FromError(RiftBot.CommandDenyMessage)
                : PreconditionResult.FromSuccess();
        }
    }
}
