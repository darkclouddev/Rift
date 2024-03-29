using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;
using Rift.Services.Message;
using Rift.Util;

using IonicLib;

using Discord;
using Discord.WebSocket;

using Rift.Services.Interfaces;

namespace Rift.Services
{
    public class RoleService : IRoleService
    {
        static Timer tempRoleTimer;

        readonly IMessageService messageService;
        
        public RoleService(IMessageService messageService)
        {
            this.messageService = messageService;
            
            // TODO: implement scheduling timer
            tempRoleTimer = new Timer(
                async delegate { await TimerProcAsync(); },
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
                await RemoveTempRoleAsync(expiredRole.UserId, expiredRole.RoleId);
        }

        public async Task<(bool, IonicMessage)> AddPermanentRoleAsync(ulong userId, ulong roleId)
        {
            if (!IonicHelper.GetGuildUserById(Settings.App.MainGuildId, userId, out var sgUser))
                return (false, MessageService.UserNotFound);

            if (!IonicHelper.GetRole(Settings.App.MainGuildId, roleId, out var role))
                return (false, MessageService.RoleNotFound);

            await sgUser.AddRoleAsync(role);
            return (true, null);
        }

        public async Task<(bool, IonicMessage)> RemovePermanentRoleAsync(ulong userId, ulong roleId)
        {
            if (!IonicHelper.GetGuildUserById(Settings.App.MainGuildId, userId, out var sgUser))
                return (false, MessageService.UserNotFound);

            if (!IonicHelper.GetRole(Settings.App.MainGuildId, roleId, out var role))
                return (false, MessageService.RoleNotFound);

            await sgUser.RemoveRoleAsync(role);
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

            if (!IonicHelper.GetGuildUserById(Settings.App.MainGuildId, userId, out var sgUser))
                return;

            if (!IonicHelper.GetRole(Settings.App.MainGuildId, roleId, out var serverRole))
                return;

            await sgUser.AddRoleAsync(serverRole);
            await DB.TempRoles.AddAsync(role);
        }

        public async Task<(bool, Embed)> RemoveTempRoleAsync(ulong userId, ulong roleId)
        {
            await DB.TempRoles.RemoveAsync(userId, roleId);

            IonicHelper.GetGuildUserById(Settings.App.MainGuildId, userId, out var sgUser);

            var role = sgUser?.Roles.FirstOrDefault(x => x.Id == roleId);

            if (role != null)
                await sgUser.RemoveRoleAsync(role);

            RiftBot.Log.Information($"Removed role {roleId.ToString()} from {sgUser} {userId.ToString()}");

            return (true, null);
        }

        public async Task<List<RiftTempRole>> GetUserTempRolesAsync(ulong userId)
        {
            return await DB.TempRoles.GetAsync(userId);
        }

        public async Task RestoreTempRolesAsync(SocketGuildUser sgUser)
        {
            RiftBot.Log.Information($"User {sgUser.ToLogString()} joined, checking temp roles");

            var tempRoles = await DB.TempRoles.GetAsync(sgUser.Id);

            if (tempRoles is null || tempRoles.Count == 0)
            {
                RiftBot.Log.Debug($"No temp roles for user {sgUser}");
                return;
            }

            var remainingRoles = tempRoles.Select(x => x.RoleId).Except(sgUser.Roles.Select(x => x.Id));

            foreach (var id in remainingRoles)
            {
                if (!IonicHelper.GetRole(Settings.App.MainGuildId, id, out var role))
                {
                    RiftBot.Log.Error($"Applying role {id.ToString()}: FAILED");
                    continue;
                }

                await sgUser.AddRoleAsync(role);
                RiftBot.Log.Debug($"Successfully added temp role \"{role.Name}\" for user {sgUser}");
            }
        }

        public async Task UpdateInventoryRoleAsync(ulong userId, int id, bool add)
        {
            if (!await DB.RoleInventory.HasAnyAsync(userId, id))
            {
                await messageService.SendMessageAsync("roleinventory-wrongnumber", Settings.ChannelId.Commands, new FormatData(userId));
                return;
            }

            var dbUser = await DB.Users.GetAsync(userId);
            if (dbUser is null || !IonicHelper.GetGuildUserById(Settings.App.MainGuildId, userId, out var sgUser))
            {
                await messageService.SendMessageAsync(MessageService.UserNotFound, Settings.ChannelId.Commands);
                return;
            }

            var role = await DB.Roles.GetAsync(id);
            if (role is null)
            {
                await messageService.SendMessageAsync(MessageService.RoleNotFound, Settings.ChannelId.Commands);
                await RiftBot.SendMessageToAdmins($"User <@{userId.ToString()}> failed to set role ID {id.ToString()}.");
                return;
            }

            if (!IonicHelper.GetRole(Settings.App.MainGuildId, role.RoleId, out var guildRole))
            {
                await messageService.SendMessageAsync(MessageService.RoleNotFound, Settings.ChannelId.Commands);
                return;
            }

            var hasRole = IonicHelper.HasRolesAny(Settings.App.MainGuildId, userId, role.RoleId);

            if (add)
            {
                if (hasRole)
                {
                    await messageService.SendMessageAsync("roleinventory-hasrole", Settings.ChannelId.Commands, new FormatData(userId));
                    return;
                }

                await sgUser.AddRoleAsync(guildRole);
            }
            else
            {
                if (!hasRole)
                {
                    await messageService.SendMessageAsync("roleinventory-norole", Settings.ChannelId.Commands, new FormatData(userId));
                    return;
                }

                await sgUser.RemoveRoleAsync(guildRole);
            }
        }

        const string InventoryIdentifier = "role-inventory-list";

        public async Task GetInventoryAsync(ulong userId)
        {
            await messageService.SendMessageAsync(InventoryIdentifier, Settings.ChannelId.Commands, new FormatData(userId))
                .ConfigureAwait(false);
        }
        
        public async Task<List<ulong>> GetNitroBoostersAsync()
        {
            var nitro = await DB.Roles.GetAsync(91);
            
            if (!IonicHelper.GetRole(Settings.App.MainGuildId, nitro.RoleId, out var role)
                || !(role is SocketRole sgRole))
                return null;

            return sgRole.Members.Select(x => x.Id).ToList();
        }
    }
}
