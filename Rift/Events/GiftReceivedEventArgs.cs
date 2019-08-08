namespace Rift.Events
{
    public class GiftReceivedEventArgs : RiftEventArgs
    {
        public ulong SenderId { get; }

        public GiftReceivedEventArgs(ulong userId, ulong senderId) : base(userId)
        {
            SenderId = senderId;
        }
    }
}
