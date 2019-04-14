namespace Rift.Data.Models
{
    public class RiftLolData
    {
        public ulong UserId { get; set; }
        public string SummonerRegion { get; set; } = "";
        public string PlayerUUID { get; set; } = "";
        public string AccountId { get; set; } = "";
        public string SummonerId { get; set; } = "";
        public string SummonerName { get; set; } = "";

        public RiftUser User { get; set; }
    }
}
