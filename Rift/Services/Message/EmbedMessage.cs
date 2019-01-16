using System;

using Discord;

namespace Rift.Services.Message
{
    public sealed class EmbedMessage : SendMessageBase
    {
        public EmbedMessage(DestinationType destType, ulong destId, TimeSpan offset, Embed embed)
        {
            MessageType = MessageType.Embed;

            DestinationType = destType;
            DestinationId = destId;
            DeliveryTime = DateTime.UtcNow + offset;
            Embed = embed;
        }
    }
}
