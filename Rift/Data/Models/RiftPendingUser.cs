using System;

namespace Rift.Data.Models
{
    public class RiftPendingUser
    {
        public UInt64 UserId { get; set; }
        public String Region { get; set; }
        public String PlayerUUID { get; set; }
        public String AccountId { get; set; }
        public String SummonedId { get; set; }
        public String ConfirmationCode { get; set; }
        public DateTime ExpirationTime { get; set; }

        public RiftUser User { get; set; }
    }
}
