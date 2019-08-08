namespace Rift.Events
{
    public class LolDataCreatedEventArgs : RiftEventArgs
    {
        public LolDataCreatedEventArgs(ulong userId) : base(userId)
        {
        }
    }
}
