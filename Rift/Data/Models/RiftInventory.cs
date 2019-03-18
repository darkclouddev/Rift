using System;

namespace Rift.Data.Models
{
    public class RiftInventory
    {
        public UInt64 UserId { get; set; }
        public UInt32 Coins { get; set; } = 0u;
        public UInt32 Tokens { get; set; } = 0u;
        public UInt32 Chests { get; set; } = 0u;
        public UInt32 Capsules { get; set; } = 0u;
        public UInt32 Spheres { get; set; } = 0u;
        public UInt32 PowerupsDoubleExp { get; set; } = 0u;
        public UInt32 PowerupsBotRespect { get; set; } = 0u;
        public UInt32 UsualTickets { get; set; } = 0u;
        public UInt32 RareTickets { get; set; } = 0u;

        public RiftUser User { get; set; }
    }
}
