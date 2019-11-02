using System.Threading.Tasks;

using Discord.WebSocket;

using Rift.Data.Models;

namespace Rift.Services.Interfaces
{
    public interface IEmoteService
    {
        Task AddEmotesFromGuild(SocketGuild guild);
        Task ReloadEmotesAsync();
        RiftMessage FormatMessage(string template, RiftMessage message);
        string GetEmoteString(string template);
    }
}
