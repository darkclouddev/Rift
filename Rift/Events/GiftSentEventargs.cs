namespace Rift.Events
{
    public class GiftSentEventArgs : RiftEventArgs
    {
        public ulong ReceiverId { get; protected set; }

        public GiftSentEventArgs(ulong userId, ulong receiverId) : base(userId)
        {
            ReceiverId = receiverId;
        }
    }
}
