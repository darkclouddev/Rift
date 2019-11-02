using System.Collections.Generic;
using System.Threading.Tasks;

using Discord;

using Rift.Data.Models;
using Rift.Services.Message;
using Rift.Services.Message.Templates;

namespace Rift.Services.Interfaces
{
    public interface IMessageService
    {
        bool TryAddSend(SendMessageBase message);
        bool TryAddDelete(DeleteMessageBase message);
        int GetSendQueueLength();
        int GetDeleteQueueLength();
        List<ITemplate> GetActiveTemplates();
        Task<IonicMessage> GetMessageAsync(string identifier, FormatData data);
        Task<IonicMessage> FormatMessageAsync(RiftMessage message, FormatData data = null);
        Task<IUserMessage> SendMessageAsync(string identifier, ulong channelId, FormatData data);
        Task<IUserMessage> SendMessageAsync(IonicMessage message, ulong channelId);
    }
}
