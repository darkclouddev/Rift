using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rift.Data.Models
{
    [Table("TempRoles")]
    public class RiftTempRole
    {
        [ForeignKey(nameof(User)), Required, Column(Order = 0)]
        public ulong UserId { get; set; }
        public RiftUser User { get; set; }

        [Column(Order = 1)]
        public ulong RoleId;

        [Column(Order = 2)]
        public DateTime ObtainedAtTimestamp;
        
        [Column(Order = 3)]
        public string ObtainedFrom;

        [Column(Order = 4)]
        public DateTime ExpirationTimestamp;

        public override int GetHashCode()
        {
            return UserId.GetHashCode() * 37 + RoleId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is RiftTempRole role
                   && UserId == role.UserId
                   && RoleId == role.RoleId;
        }
    }
}
