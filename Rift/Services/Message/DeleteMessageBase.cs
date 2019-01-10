using System;

namespace Rift.Services.Message
{
    public abstract class DeleteMessageBase
    {
        public Guid Id = Guid.NewGuid();

        public virtual ulong ChannelId { get; protected set; }

        public virtual ulong MessageId { get; protected set; }

        public virtual DateTime DeletionTime { get; protected set; }
    }
}
