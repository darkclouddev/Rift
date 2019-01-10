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
            DeletionTime = DateTime.Now + offset;
        }

        public DeleteMessage(IUserMessage message, TimeSpan offset)
        {
            ChannelId = message.Channel.Id;
            MessageId = message.Id;
            DeletionTime = DateTime.Now + offset;
        }
    }
}
