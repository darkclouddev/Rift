using System.Threading.Tasks;

using Discord;

using Rift.Services.Message;

namespace Rift.Services.Interfaces
{
    public interface IModerationService
    {
        Task KickAsync(IUser target, string reason, IUser moderator);
        Task BanAsync(IUser target, string reason, IUser moderator);
        Task MuteAsync(IUser target, string reason, string time, IUser moderator);
        Task UnmuteAsync(IUser target, string reason, IUser moderator);
        Task WarnAsync(IUser target, string reason, IUser moderator);
        Task<IonicMessage> GetUserActionLogsAsync(IUser user);
        Task<IonicMessage> GetLastActionsAsync();
    }
}
