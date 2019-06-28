namespace Rift.Events
{
    public class GiftReceivedEventArgs : RiftEventArgs
    {
        public ulong SenderId { get; protected set; }

        public GiftReceivedEventArgs(ulong userId, ulong senderId) : base(userId)
        {
            SenderId = senderId;
        }
    }
}
