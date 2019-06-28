namespace Rift.Events
{
    public class LevelReachedEventArgs : RiftEventArgs
    {
        public uint Level { get; set; }

        public LevelReachedEventArgs(ulong userId, uint level) : base(userId)
        {
            Level = level;
        }
    }
}
