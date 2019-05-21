using System;

using Discord;

namespace Rift.Services.Message
{
    public sealed class MixedMessage : SendMessageBase
    {
        public MixedMessage(DestinationType destType, ulong destId, TimeSpan offset, string text, Embed embed, string imageUrl)
        {
            MessageType = MessageType.Mixed;

            DestinationType = destType;
            DestinationId = destId;
            DeliveryTime = DateTime.UtcNow + offset;
            Text = text;
            Embed = embed;
            ImageUrl = imageUrl;
        }

        public MixedMessage(DestinationType destType, ulong destId, TimeSpan offset, IonicMessage message)
        {
            MessageType = MessageType.Mixed;

            DestinationType = destType;
            DestinationId = destId;
            DeliveryTime = DateTime.UtcNow + offset;
            Text = message.Text ?? "";
            Embed = message.Embed;
            ImageUrl = message.ImageUrl;
        }
    }
}
