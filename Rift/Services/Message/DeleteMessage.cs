using System;

using Discord;

namespace Rift.Services.Message
{
    public sealed class DeleteMessage : DeleteMessageBase
    {
        public DeleteMessage(ulong channelId, ulong messageId, TimeSpan offset)
        {
            ChannelId = channelId;
            MessageId = messageId;
            DeletionTime = DateTime.UtcNow + offset;
        }

        public DeleteMessage(IUserMessage message, TimeSpan offset)
        {
            ChannelId = message.Channel.Id;
            MessageId = message.Id;
            DeletionTime = DateTime.UtcNow + offset;
        }
    }
}
