using System.Threading.Tasks;

using Rift.Preconditions;
using Rift.Services;

using Discord;
using Discord.Commands;

namespace Rift.Modules
{
    public class ModerationModule : RiftModuleBase
    {
        readonly ModerationService moderationService;

        public ModerationModule(ModerationService moderationService)
        {
            this.moderationService = moderationService;
        }

        [Command("kick")]
        [RequireModerator]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task KickAsync(IUser user, [Remainder] string reason)
        {
            await moderationService.KickAsync(user, reason, Context.User).ConfigureAwait(false);
        }

        [Command("ban")]
        [RequireModerator]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanAsync(IUser user, [Remainder] string reason)
        {
            await moderationService.BanAsync(user, reason, Context.User).ConfigureAwait(false);
        }

        [Command("mute")]
        [RequireModerator]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task MuteAsync(IUser user, string time, string reason)
        {
            await moderationService.MuteAsync(user, reason, time, Context.User).ConfigureAwait(false);
        }

        [Command("unmute")]
        [RequireModerator]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task UnmuteAsync(IUser user, string reason)
        {
            await moderationService.UnmuteAsync(user, reason, Context.User).ConfigureAwait(false);
        }

        [Command("warn")]
        [RequireModerator]
        [RequireContext(ContextType.Guild)]
        public async Task WarnAsync(IUser user, string reason)
        {
            await moderationService.WarnAsync(user, reason, Context.User).ConfigureAwait(false);
        }
    }
}
