using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rift.Data.Models
{
    public class RiftUser
    {
        [Key, Required, Column(Order = 0)]
        public ulong UserId { get; set; }

        [Column(Order = 1)]
        public uint Experience { get; set; } = 0u;

        [Column(Order = 2)]
        public uint Level { get; set; } = 0u;

        [Column(Order = 3)]
        public decimal Donate { get; set; } = 0m;
    }
}
