namespace Rift.Events
{
    public class CoinsSpentEventArgs : RiftEventArgs
    {
        public uint Amount { get; }

        public CoinsSpentEventArgs(ulong userId, uint amount) : base(userId)
        {
            Amount = amount;
        }
    }
}
