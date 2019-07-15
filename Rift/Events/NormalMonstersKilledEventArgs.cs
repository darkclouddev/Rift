namespace Rift.Events
{
    public class NormalMonstersKilledEventArgs : RiftEventArgs
    {
        public uint Amount { get; }
        
        public NormalMonstersKilledEventArgs(ulong userId, uint amount) : base(userId)
        {
            Amount = amount;
        }
    }
}
