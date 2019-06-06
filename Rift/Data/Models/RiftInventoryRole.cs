using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rift.Data.Models
{
    public class RiftInventoryRole
    {
        public ulong UserId { get; set; }
        public ulong RoleId { get; set; }
        public DateTime ObtainedAt { get; set; }
        public string ObtainedFrom { get; set; }

        public RiftUser User { get; set; }

        public override int GetHashCode()
        {
            return UserId.GetHashCode() * 37 + RoleId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is RiftInventoryRole role
                   && UserId == role.UserId
                   && RoleId == role.RoleId;
        }

        public override string ToString()
        {
            return $"{nameof(RiftInventoryRole)}: {nameof(UserId)}: {UserId.ToString()}, {nameof(RoleId)}: {RoleId.ToString()}";
        }
    }
}
