namespace Rift.Events
{
    public class MessageCreatedEventArgs : RiftEventArgs
    {
        public uint TotalMessages { get; set; }

        public MessageCreatedEventArgs(ulong userId, uint totalMessages) : base(userId)
        {
            TotalMessages = totalMessages;
        }
    }
}
