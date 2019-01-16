using System;

using Discord;

namespace Rift.Services.Message
{
    public sealed class MixedMessage : SendMessageBase
    {
        public MixedMessage(DestinationType destType, ulong destId, TimeSpan offset, string text, Embed embed)
        {
            MessageType = MessageType.Mixed;

            DestinationType = destType;
            DestinationId = destId;
            DeliveryTime = DateTime.UtcNow + offset;
            Text = text;
            Embed = embed;
        }
    }
}
