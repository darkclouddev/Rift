namespace Rift.Events
{
    public class GiftedModeratorEventArgs : RiftEventArgs
    {
        public ulong ReceiverId { get; }

        public GiftedModeratorEventArgs(ulong userId, ulong receiverId) : base(userId)
        {
            ReceiverId = receiverId;
        }
    }
}
