namespace Rift.Events
{
    public class GiftedDeveloperEventArgs : RiftEventArgs
    {
        public ulong ReceiverId { get; }

        public GiftedDeveloperEventArgs(ulong userId, ulong receiverId) : base(userId)
        {
            ReceiverId = receiverId;
        }
    }
}
