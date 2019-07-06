namespace Rift.Events
{
    public class GiveawaysParticipatedEventArgs : RiftEventArgs
    {
        public GiveawaysParticipatedEventArgs(ulong userId) : base(userId)
        {
        }
    }
}