using System;

using Humanizer;

namespace Rift.Data.Models
{
    public class RiftTempRole
    {
        public UInt64 UserId { get; set; }
        public UInt64 RoleId { get; set; }
        public DateTime ObtainedTime { get; set; }
        public String ObtainedFrom { get; set; }
        public DateTime ExpirationTime { get; set; }

        public RiftUser User { get; set; }

        public override Int32 GetHashCode()
        {
            return UserId.GetHashCode() * 37 + RoleId.GetHashCode();
        }

        public override Boolean Equals(Object obj)
        {
            return obj is RiftTempRole role
                   && UserId == role.UserId
                   && RoleId == role.RoleId;
        }

        public override String ToString()
        {
            return $"RiftTempRole: {nameof(UserId)}: {UserId.ToString()}, {nameof(RoleId)}: {RoleId.ToString()}, {nameof(ExpirationTime)}: {ExpirationTime.Humanize()}";
        }
    }
}
