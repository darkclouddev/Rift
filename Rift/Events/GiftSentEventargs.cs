namespace Rift.Events
{
    public class GiftSentEventArgs : RiftEventArgs
    {
        public ulong ReceiverId { get; }

        public GiftSentEventArgs(ulong userId, ulong receiverId) : base(userId)
        {
            ReceiverId = receiverId;
        }
    }
}
