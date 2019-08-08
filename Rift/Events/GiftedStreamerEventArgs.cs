namespace Rift.Events
{
    public class GiftedStreamerEventArgs : RiftEventArgs
    {
        public ulong ReceiverId { get; }

        public GiftedStreamerEventArgs(ulong userId, ulong receiverId) : base(userId)
        {
            ReceiverId = receiverId;
        }
    }
}
