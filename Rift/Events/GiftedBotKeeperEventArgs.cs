namespace Rift.Events
{
    public class GiftedBotKeeperEventArgs : RiftEventArgs
    {
        public ulong ReceiverId { get; protected set; }

        public GiftedBotKeeperEventArgs(ulong userId, ulong receiverId) : base(userId)
        {
            ReceiverId = receiverId;
        }
    }
}
