using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Rift.Services;

using IonicLib.Extensions;

using Humanizer;

namespace Rift.Rewards
{
    public class RandomRoleReward
    {
        public ulong RoleId;
        public string RoleString;

        protected async Task GetRole(List<ulong> AvailableRoles, ulong userId)
        {
            var tempRoles = (await RiftBot.GetService<RoleService>().GetUserTempRolesAsync(userId)).Select(x => x.RoleId);

            var availableRoles = AvailableRoles.Except(tempRoles).ToList();
            if (availableRoles.Any())
            {
                RoleId = availableRoles.Random();
                return;
            }

            RoleId = 0ul;
        }

        protected async Task GiveRoleAsync(ulong userId, uint days)
        {
            if (RoleId != 0)
            {
                await RiftBot.GetService<RoleService>().AddTempRoleAsync(userId, RoleId, TimeSpan.FromDays(days), nameof(RandomRoleReward).Humanize());
            }
        }
    }
}
