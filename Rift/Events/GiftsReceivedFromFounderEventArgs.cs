namespace Rift.Events
{
    public class GiftsReceivedFromFounderEventArgs : RiftEventArgs
    {
        public ulong SenderId { get; }

        public GiftsReceivedFromFounderEventArgs(ulong userId, ulong senderId) : base(userId)
        {
            SenderId = senderId;
        }
    }
}
