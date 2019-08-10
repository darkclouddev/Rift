using System;
using System.Threading.Tasks;

using Discord.Commands;

using IonicLib;

using Rift.Configuration;

namespace Rift.Preconditions
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequireTicketKeeperAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var role = await DB.Roles.GetAsync(173);
            
            if (IonicClient.HasRolesAny(Settings.App.MainGuildId, context.User.Id, role.RoleId) || RiftBot.IsAdmin(context.User))
                return PreconditionResult.FromSuccess();

            return PreconditionResult.FromError(RiftBot.CommandDenyMessage);
        }
    }
}
