namespace Rift.Data.Models
{
    public class RiftInventory
    {
        public ulong UserId { get; set; }
        public uint Coins { get; set; } = 0u;
        public uint Tokens { get; set; } = 0u;
        public uint Chests { get; set; } = 0u;
        public uint Capsules { get; set; } = 0u;
        public uint Spheres { get; set; } = 0u;
        public uint PowerupsDoubleExp { get; set; } = 0u;
        public uint PowerupsBotRespect { get; set; } = 0u;
        public uint UsualTickets { get; set; } = 0u;
        public uint RareTickets { get; set; } = 0u;

        public RiftUser User { get; set; }
    }
}
