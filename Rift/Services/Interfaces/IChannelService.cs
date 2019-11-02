using System.Threading.Tasks;

using Discord;

namespace Rift.Services.Interfaces
{
    public interface IChannelService
    {
        Task DenyAccessToUserAsync(IUser roomOwnerUser, IUser targetUser);
    }
}
