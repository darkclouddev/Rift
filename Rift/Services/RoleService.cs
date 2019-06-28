using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;
using Rift.Services.Message;

using IonicLib;

using Discord;
using Discord.WebSocket;

namespace Rift.Services
{
    public class RoleService
    {
        static Timer tempRoleTimer;

        public RoleService()
        {
            tempRoleTimer = new Timer(
                async delegate
                {
                    await TimerProcAsync();
                },
                null,
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(5));
        }

        async Task TimerProcAsync()
        {
            var expiredRoles = await DB.TempRoles.GetExpiredTempRolesAsync();

            if (expiredRoles is null || expiredRoles.Count == 0)
                return;

            foreach (var expiredRole in expiredRoles)
            {
                await RemoveTempRoleAsync(expiredRole.UserId, expiredRole.RoleId);
            }
        }

        public async Task<(bool, IonicMessage)> AddPermanentRoleAsync(ulong userId, ulong roleId)
        {
            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (sgUser is null)
                return (false, MessageService.UserNotFound);

            if (!IonicClient.GetRole(Settings.App.MainGuildId, roleId, out var role))
                return (false, MessageService.RoleNotFound);

            await sgUser.AddRoleAsync(role);
            return (true, null);
        }

        public async Task AddTempRoleAsync(ulong userId, ulong roleId, TimeSpan duration, string reason)
        {
            var role = new RiftTempRole
            {
                UserId = userId,
                RoleId = roleId,
                ObtainedFrom = reason,
                ObtainedTime = DateTime.UtcNow,
                ExpirationTime = DateTime.UtcNow + duration,
            };

            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            if (sgUser is null)
                return;

            if (!IonicClient.GetRole(Settings.App.MainGuildId, roleId, out var serverRole))
                return;

            await sgUser.AddRoleAsync(serverRole);
            await DB.TempRoles.AddAsync(role);
        }

        public async Task<(bool, Embed)> RemoveTempRoleAsync(ulong userId, ulong roleId)
        {
            await DB.TempRoles.RemoveAsync(userId, roleId);

            var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

            var role = sgUser?.Roles.FirstOrDefault(x => x.Id == roleId);

            if (role != null)
                await sgUser.RemoveRoleAsync(role);

            RiftBot.Log.Info($"Removed role {roleId.ToString()} from {sgUser} {userId.ToString()}");

            return (true, null);
        }

        public async Task<List<RiftTempRole>> GetUserTempRolesAsync(ulong userId)
        {
            return await DB.TempRoles.GetAsync(userId);
        }

        public async Task RestoreTempRolesAsync(SocketGuildUser sgUser)
        {
            RiftBot.Log.Info($"User {sgUser} ({sgUser.Id.ToString()}) joined, checking temp roles");

            var tempRoles = await DB.TempRoles.GetAsync(sgUser.Id);

            if (tempRoles is null || tempRoles.Count == 0)
            {
                RiftBot.Log.Debug($"No temp roles for user {sgUser}");
                return;
            }

            foreach (var tempRole in tempRoles)
            {
                if (sgUser.Roles.Any(x => x.Id == tempRole.RoleId))
                    continue;

                if (!IonicClient.GetRole(Settings.App.MainGuildId, tempRole.RoleId, out var role))
                {
                    RiftBot.Log.Error($"Applying role {tempRole.RoleId.ToString()}: FAILED");
                    continue;
                }

                await sgUser.AddRoleAsync(role);
                RiftBot.Log.Debug($"Successfully added temp role \"{role.Name}\" for user {sgUser}");
            }
        }
    }
}
