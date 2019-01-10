namespace Rift.Data.Models.LolData
{
    public class PendingUser
    {
        public ulong UserId { get; set; }
        public string Region { get; set; }
        public string PlayerUUID { get; set; }
        public string AccountId { get; set; }
        public string SummonedId { get; set; }
        public string ConfirmationCode { get; set; }
        public ulong ExpirationTimestamp { get; set; }
    }
}
