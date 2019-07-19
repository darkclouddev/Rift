namespace Rift.Data.Models
{
    public class RiftVote
    {
        public ulong UserId { get; set; }
        public int CommunityId { get; set; } = 0;
        public int TeamId { get; set; } = 0;
        public ulong StreamerId { get; set; } = 0;

        public RiftUser User { get; set; }
    }
}
