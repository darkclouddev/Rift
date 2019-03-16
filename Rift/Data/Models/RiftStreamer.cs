using System;

namespace Rift.Data.Models
{
    public class RiftStreamer
    {
        public UInt64 UserId { get; set; }
        public String PictureUrl { get; set; }
        public String StreamUrl { get; set; }
        
        public RiftUser User { get; set; }
    }
}