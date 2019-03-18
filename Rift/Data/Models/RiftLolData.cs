using System;

namespace Rift.Data.Models
{
    public class RiftLolData
    {
        public UInt64 UserId { get; set; }
        public String SummonerRegion { get; set; } = "";
        public String PlayerUUID { get; set; } = "";
        public String AccountId { get; set; } = "";
        public String SummonerId { get; set; } = "";
        public String SummonerName { get; set; } = "";

        public RiftUser User { get; set; }
    }
}
