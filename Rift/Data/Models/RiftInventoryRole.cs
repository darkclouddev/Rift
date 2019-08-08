using System;

namespace Rift.Data.Models
{
    public class RiftRoleInventory
    {
        public ulong UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime ObtainedAt { get; set; }
        public string ObtainedFrom { get; set; }

        public RiftUser User { get; set; }

        public override int GetHashCode()
        {
            return UserId.GetHashCode() * 37 + RoleId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is RiftRoleInventory role
                   && UserId == role.UserId
                   && RoleId == role.RoleId;
        }

        public override string ToString()
        {
            return
                $"{nameof(RiftRoleInventory)}: {nameof(UserId)}: {UserId.ToString()}, {nameof(RoleId)}: {RoleId.ToString()}";
        }
    }
}
