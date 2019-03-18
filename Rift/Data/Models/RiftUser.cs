using System;
using System.Collections.Generic;

namespace Rift.Data.Models
{
    public class RiftUser
    {
        public UInt64 UserId { get; set; }
        public UInt32 Experience { get; set; } = 0u;
        public UInt32 Level { get; set; } = 0u;
        public Decimal Donate { get; set; } = 0m;

        public RiftCooldowns Cooldowns { get; set; }
        public RiftInventory Inventory { get; set; }
        public RiftLolData LolData { get; set; }
        public RiftPendingUser PendingUser { get; set; }
        public ICollection<RiftTempRole> TempRoles { get; set; }
        public RiftStatistics Statistics { get; set; }
        public RiftStreamer Streamers { get; set; }
    }
}
