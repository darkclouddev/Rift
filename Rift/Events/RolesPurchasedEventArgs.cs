namespace Rift.Events
{
    public class RolesPurchasedEventArgs : RiftEventArgs
    {
        public uint Amount { get; }

        public RolesPurchasedEventArgs(ulong userId, uint amount) : base(userId)
        {
            Amount = amount;
        }
    }
}
