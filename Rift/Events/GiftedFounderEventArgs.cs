namespace Rift.Events
{
    public class GiftedFounderEventArgs : RiftEventArgs
    {
        public ulong SenderId { get; }

        public GiftedFounderEventArgs(ulong userId, ulong senderId) : base(userId)
        {
            SenderId = senderId;
        }
    }
}
