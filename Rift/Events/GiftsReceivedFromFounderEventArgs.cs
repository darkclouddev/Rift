namespace Rift.Events
{
    public class GiftsReceivedFromFounderEventArgs : RiftEventArgs
    {
        public ulong SenderId { get; protected set; }

        public GiftsReceivedFromFounderEventArgs(ulong userId, ulong senderId) : base(userId)
        {
            SenderId = senderId;
        }
    }
}
