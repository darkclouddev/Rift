using System;

namespace Rift.Services.Message
{
    public sealed class PlainTextMessage : SendMessageBase
    {
        public PlainTextMessage(DestinationType destType, ulong destId, TimeSpan timeOffset, string text)
        {
            MessageType = MessageType.PlainText;

            DestinationType = destType;
            DestinationId = destId;
            DeliveryTime = DateTime.UtcNow + timeOffset;
            Text = text;
        }
    }
}
