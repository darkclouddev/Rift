using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rift.Data.Models
{
    public class RiftInventory
    {
        [ForeignKey(nameof(User)), Required, Column(Order = 0)]
        public ulong UserId { get; set; }
        public RiftUser User { get; set; }

        [Column(Order = 1)]
        public uint Coins { get; set; } = 0u;

        [Column(Order = 2)]
        public uint Tokens { get; set; } = 0u;

        [Column(Order = 3)]
        public uint Chests { get; set; } = 0u;

        [Column(Order = 4)]
        public uint Capsules { get; set; } = 0u;

        [Column(Order = 5)]
        public uint Spheres { get; set; } = 0u;

        [Column(Order = 6)]
        public uint PowerupsDoubleExp { get; set; } = 0u;

        [Column(Order = 7)]
        public uint PowerupsBotRespect { get; set; } = 0u;

        [Column(Order = 8)]
        public uint UsualTickets { get; set; } = 0u;

        [Column(Order = 9)]
        public uint RareTickets { get; set; } = 0u;
    }
}
