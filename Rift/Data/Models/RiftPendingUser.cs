using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rift.Data.Models
{
    public class RiftPendingUser
    {
        [ForeignKey(nameof(User)), Required, Column(Order = 0)]
        public ulong UserId { get; set; }
        public RiftUser User { get; set; }

        [Column(Order = 1)]
        public string Region { get; set; }

        [Column(Order = 2)]
        public string PlayerUUID { get; set; }

        [Column(Order = 3)]
        public string AccountId { get; set; }

        [Column(Order = 4)]
        public string SummonedId { get; set; }

        [Column(Order = 5)]
        public string ConfirmationCode { get; set; }

        [Column(Order = 6)]
        public ulong ExpirationTimestamp { get; set; }
    }
}
