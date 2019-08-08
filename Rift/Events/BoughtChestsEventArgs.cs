namespace Rift.Events
{
    public class BoughtChestsEventArgs : RiftEventArgs
    {
        public uint Amount { get; }

        public BoughtChestsEventArgs(ulong userId, uint amount) : base(userId)
        {
            Amount = amount;
        }
    }
}
