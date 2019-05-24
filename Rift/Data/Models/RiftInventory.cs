namespace Rift.Data.Models
{
    public class RiftInventory
    {
        public ulong UserId { get; set; }

        public uint Coins { get; set; } = 0u;

        public uint Tokens { get; set; } = 0u;

        public uint Essence { get; set; } = 0u;

        public uint Chests { get; set; } = 0u;

        public uint Spheres { get; set; } = 0u;

        public uint Capsules { get; set; } = 0u;

        public uint Tickets { get; set; } = 0u;

        public uint BonusDoubleExp { get; set; } = 0u;

        public uint BonusBotRespect { get; set; } = 0u;

        public uint BonusRewind { get; set; } = 0u;

        public RiftUser User { get; set; }
    }
}
