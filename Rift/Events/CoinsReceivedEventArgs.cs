namespace Rift.Events
{
    public class CoinsReceivedEventArgs : RiftEventArgs
    {
        public uint Amount { get; }

        public CoinsReceivedEventArgs(ulong userId, uint amount) : base(userId)
        {
            Amount = amount;
        }
    }
}
