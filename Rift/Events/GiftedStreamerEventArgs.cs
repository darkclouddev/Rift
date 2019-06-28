namespace Rift.Events
{
    public class GiftedStreamerEventArgs : RiftEventArgs
    {
        public ulong ReceiverId { get; protected set; }

        public GiftedStreamerEventArgs(ulong userId, ulong receiverId) : base(userId)
        {
            ReceiverId = receiverId;
        }
    }
}
