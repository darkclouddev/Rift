using System;

namespace Rift.Data.Models
{
    public class RiftToxicity
    {
        public ulong UserId { get; set; }
        public uint Percent { get; set; }
        public DateTime LastUpdated { get; set; }

        public uint Level
        {
            get
            {
                if (Percent == 0u)
                    return 0u;

                if (Percent < 50u)
                    return 1u;

                return 2u;
            }
        }

        public RiftUser User { get; set; }
    }
}
