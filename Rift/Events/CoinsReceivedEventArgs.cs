namespace Rift.Events
{
    public class CoinsReceivedEventArgs : RiftEventArgs
    {
        public uint Amount { get; protected set; }

        public CoinsReceivedEventArgs(ulong userId, uint amount) : base(userId)
        {
            Amount = amount;
        }
    }
}
