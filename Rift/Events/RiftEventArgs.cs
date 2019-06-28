using System;

namespace Rift.Events
{
    public class RiftEventArgs : EventArgs
    {
        public ulong UserId { get; protected set; }

        public RiftEventArgs(ulong userId)
        {
            UserId = userId;
        }
    }
}
