using System;

namespace Rift.Services.Reward
{
    public class RoleReward : RewardBase
    {
        public int RoleId { get; set; }
        public TimeSpan? Duration { get; set; }

        public RoleReward()
        {
            Type = RewardType.Role;
        }

        public RoleReward SetRole(int roleId)
        {
            RoleId = roleId;
            return this;
        }

        public RoleReward SetDuration(TimeSpan duration)
        {
            Duration = duration;
            return this;
        }
    }
}
