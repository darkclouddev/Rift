using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

using Rift.Services;
using Rift.Services.Role;

using IonicLib.Extensions;

namespace Rift.Rewards
{
    public class RandomRoleReward
    {
        public ulong RoleId;
        public string RoleString;

        protected void GetRole(List<ulong> AvailableRoles, ulong userId)
        {
            var tempRoles = RiftBot.GetService<RoleService>().GetTempRoles(userId).Select(x => x.RoleId);

            var availableRoles = AvailableRoles.Except(tempRoles);
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
                var role = new TempRole(userId, RoleId, TimeSpan.FromDays(days));
                await RiftBot.GetService<RoleService>().AddTempRoleAsync(role);
            }
        }
    }
}
