namespace Rift.Events
{
    public class RareMonstersKilledEventArgs : RiftEventArgs
    {
        public uint Amount { get; }
        
        public RareMonstersKilledEventArgs(ulong userId, uint amount) : base(userId)
        {
            Amount = amount;
        }
    }
}
