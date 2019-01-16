using System;

namespace Rift.Data.Models
{
    public class RiftTempRole
    {
        public ulong UserId { get; set; }
        public ulong RoleId { get; set; }
        public DateTime ObtainedTime { get; set; }
        public string ObtainedFrom { get; set; }
        public DateTime ExpirationTime { get; set; }

        public RiftUser User { get; set; }

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

        public override string ToString()
        {
            return $"RiftTempRole: {nameof(UserId)}: {UserId}, {nameof(RoleId)}: {RoleId}, {nameof(ExpirationTime)}: {ExpirationTime.ToString()}";
        }
    }
}
