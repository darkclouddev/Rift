using System.Collections.Generic;

namespace Rift.Data.Models
{
    public class RiftUser
    {
        public ulong UserId { get; set; }
        public uint Experience { get; set; } = 0u;
        public uint Level { get; set; } = 0u;
        public int ProfileBackground { get; set; } = 0;

        public RiftCooldowns Cooldowns { get; set; }
        public RiftInventory Inventory { get; set; }
        public RiftLeagueData LolData { get; set; }
        public RiftPendingUser PendingUser { get; set; }
        public ICollection<RiftTempRole> TempRoles { get; set; }
        public RiftStatistics Statistics { get; set; }
        public RiftStreamer Streamers { get; set; }
        public RiftToxicity Toxicity { get; set; }
        public RiftBackgroundInventory BackgroundInventory { get; set; }
    }
}
