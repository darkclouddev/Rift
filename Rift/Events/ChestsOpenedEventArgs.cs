namespace Rift.Events
{
    public class ChestsOpenedEventArgs : RiftEventArgs
    {
        public uint Amount { get; set; }

        public ChestsOpenedEventArgs(ulong userId, uint amount)
            : base(userId)
        {
            Amount = amount;
        }
    }
}
