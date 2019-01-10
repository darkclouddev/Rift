using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;
using Rift.Services.Message;
using Rift.Services.Role;

using IonicLib;
using IonicLib.Extensions;
using IonicLib.Util;

using Discord;
using Discord.WebSocket;

using Newtonsoft.Json;

namespace Rift.Services
{
    public class RoleService
    {
        static IEnumerable<TempRole> rolesToRemove = new List<TempRole>();
        static readonly TimeSpan tempRoleCheckTimerCooldown = TimeSpan.FromSeconds(15);

        static Timer tempRoleCheckTimer;

        public RoleService()
        {
            //tempRoleCheckTimer = new Timer(async delegate { await CheckExpiredRolesAsync(); }, null, TimeSpan.FromSeconds(15), tempRoleCheckTimerCooldown);
        }

        void ResetTimer(TimeSpan delay, TimeSpan period)
        {
            //tempRoleCheckTimer = new Timer(async delegate { await CheckExpiredRolesAsync(); }, null, delay, period);
        }

        public async Task AddTempRoleAsync(ulong userId, ulong roleId, TimeSpan duration, string reason)
        {
            var role = new RiftTempRole
            {
                UserId = userId,
                RoleId = roleId,
                ObtainedFrom = reason,
                ObtainedAtTimestamp = DateTime.UtcNow,
                ExpirationTimestamp = DateTime.UtcNow + duration,
            };

            await RiftBot.GetService<DatabaseService>().AddTempRoleAsync(role);
        }
    }
}
