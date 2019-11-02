using System;
using System.Threading.Tasks;

using Discord.WebSocket;

using Rift.Events;
using Rift.Services.Message;

namespace Rift.Services.Interfaces
{
    public interface IGiftService
    {
        event EventHandler<GiftSentEventArgs> GiftSent;
        event EventHandler<GiftReceivedEventArgs> GiftReceived;
        event EventHandler<GiftedFounderEventArgs> GiftedFounder;
        event EventHandler<GiftedDeveloperEventArgs> GiftedDeveloper;
        event EventHandler<GiftedModeratorEventArgs> GiftedModerator;
        event EventHandler<GiftedStreamerEventArgs> GiftedStreamer;
        Task SendDescriptionAsync();
        Task<IonicMessage> SendGiftAsync(SocketGuildUser fromSgUser, SocketGuildUser toSgUser);
    }
}
