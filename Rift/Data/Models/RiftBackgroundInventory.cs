namespace Rift.Data.Models
{
    public class RiftBackgroundInventory
    {
        public ulong UserId { get; set; }
        public int BackgroundId { get; set; }

        public RiftUser User { get; set; }

        public override bool Equals(object obj)
        {
            return obj is RiftBackgroundInventory inv
                   && UserId == inv.UserId
                   && BackgroundId == inv.BackgroundId;
        }

        public override int GetHashCode()
        {
            return UserId.GetHashCode() * 37 + BackgroundId.GetHashCode();
        }
    }
}
