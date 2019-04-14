namespace Rift.Data.Models
{
    public class RiftStreamer
    {
        public ulong UserId { get; set; }
        public string PictureUrl { get; set; }
        public string StreamUrl { get; set; }
        
        public RiftUser User { get; set; }
    }
}