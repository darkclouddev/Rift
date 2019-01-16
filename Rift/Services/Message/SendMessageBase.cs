using System;

using Discord;

namespace Rift.Services.Message
{
    public abstract class SendMessageBase
    {
        public Guid Id = Guid.NewGuid();

        public DateTime AddedOn = DateTime.UtcNow;

        public virtual MessageType MessageType { get; protected set; }

        public virtual DestinationType DestinationType { get; protected set; }

        public virtual ulong DestinationId { get; protected set; }

        public virtual DateTime DeliveryTime { get; protected set; }

        public virtual string Text { get; protected set; } = "";

        public virtual Embed Embed { get; protected set; } = null;
    }

    public enum MessageType
    {
        PlainText,
        Embed,
        Mixed,
    }

    public enum DestinationType
    {
        DM,
        GuildChannel,
    }
}
