using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using Rift.Data.Models;
using Rift.Services.Message;

namespace Rift.Services.Interfaces
{
    public interface IRoleService
    {
        Task<(bool, IonicMessage)> AddPermanentRoleAsync(ulong userId, ulong roleId);
        Task<(bool, IonicMessage)> RemovePermanentRoleAsync(ulong userId, ulong roleId);
        Task AddTempRoleAsync(ulong userId, ulong roleId, TimeSpan duration, string reason);
        Task<(bool, Embed)> RemoveTempRoleAsync(ulong userId, ulong roleId);
        Task<List<RiftTempRole>> GetUserTempRolesAsync(ulong userId);
        Task RestoreTempRolesAsync(SocketGuildUser sgUser);
        Task UpdateInventoryRoleAsync(ulong userId, int id, bool add);
        Task GetInventoryAsync(ulong userId);
        Task<List<ulong>> GetNitroBoostersAsync();
    }
}
