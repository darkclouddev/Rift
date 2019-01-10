using System;

using IonicLib.Extensions;

namespace Rift.Services.Role
{
    public class TempRole
    {
        public ulong UserId { get; set; }
        public ulong RoleId { get; set; }
        public TimeSpan Duration { get; set; }
        public ulong UntilTimestamp { get; set; }

        public TempRole(ulong userId, ulong roleId, TimeSpan duration)
        {
            UserId = userId;
            RoleId = roleId;
            Duration = duration;
            UntilTimestamp = Helper.CurrentUnixTimestamp + (ulong) duration.TotalSeconds;
        }

        public override int GetHashCode()
        {
            return UserId.GetHashCode() * 37 + RoleId.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return other is TempRole otherRole
                   && UserId == otherRole.UserId
                   && RoleId == otherRole.RoleId;
        }
    }
}
