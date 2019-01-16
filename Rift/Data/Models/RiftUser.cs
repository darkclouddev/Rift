using System.Collections.Generic;

namespace Rift.Data.Models
{
    public class RiftUser
    {
        public ulong UserId { get; set; }
        public uint Experience { get; set; } = 0u;
        public uint Level { get; set; } = 0u;
        public decimal Donate { get; set; } = 0m;

        public RiftAchievements Achievements { get; set; }
        public RiftCooldowns Cooldowns { get; set; }
        public RiftInventory Inventory { get; set; }
        public RiftLolData LolData { get; set; }
        public RiftPendingUser PendingUser { get; set; }
        public ICollection<RiftTempRole> TempRoles { get; set; }
        public RiftStatistics Statistics { get; set; }
    }
}
