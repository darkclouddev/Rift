namespace Rift.Events
{
    public class EpicMonstersKilledEventArgs : RiftEventArgs
    {
        public uint Amount { get; }
        
        public EpicMonstersKilledEventArgs(ulong userId, uint amount) : base(userId)
        {
            Amount = amount;
        }
    }
}
