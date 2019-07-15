namespace Rift.Events
{
    public class GiftedBotKeeperEventArgs : RiftEventArgs
    {
        public ulong ReceiverId { get; }

        public GiftedBotKeeperEventArgs(ulong userId, ulong receiverId) : base(userId)
        {
            ReceiverId = receiverId;
        }
    }
}
