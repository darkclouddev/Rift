namespace Rift.Events
{
    public class UserCreatedEventArgs : RiftEventArgs
    {
        public UserCreatedEventArgs(ulong userId) : base(userId)
        {
        }
    }
}
