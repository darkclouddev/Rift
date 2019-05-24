using System;
using System.Threading.Tasks;

using Discord;

namespace Rift.Services.Reward
{
    public class RoleReward : RewardBase
    {
        public ulong RoleId { get; private set; }
        public TimeSpan? Duration { get; private set; } = null;

        public RoleReward()
        {
            Type = RewardType.Role;
        }

        public RoleReward SetRole(ulong roleId)
        {
            RoleId = roleId;
            return this;
        }

        public RoleReward SetRole(IRole role)
        {
            RoleId = role.Id;
            return this;
        }

        public RoleReward SetDuration(TimeSpan duration)
        {
            Duration = duration;
            return this;
        }

        public override async Task DeliverToAsync(ulong userId)
        {
            var roleService = RiftBot.GetService<RoleService>();

            if (Duration is null)
            {
                await roleService.AddPermanentRoleAsync(userId, RoleId);
            }
            else
            {
                await roleService.AddTempRoleAsync(userId, RoleId, Duration.Value, nameof(RoleReward));
            }
        }
    }
}
