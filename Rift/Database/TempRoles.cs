using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Rift.Data;
using Rift.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace Rift.Database
{
    public class TempRoles
    {
        public async Task AddAsync(RiftTempRole role)
        {
            await DB.Users.EnsureExistsAsync(role.UserId);

            await using var context = new RiftContext();
            await context.TempRoles.AddAsync(role);
            await context.SaveChangesAsync();
        }

        public async Task<List<RiftTempRole>> GetAsync(ulong userId)
        {
            await using var context = new RiftContext();
            return await context.TempRoles
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<RiftTempRole> GetNearestExpiringTempRoleAsync()
        {
            await using var context = new RiftContext();
            return await context.TempRoles
                .Where(x => x.ExpirationTime >= DateTime.UtcNow)
                .OrderBy(x => x.ExpirationTime)
                .FirstOrDefaultAsync();
        }

        public async Task<List<RiftTempRole>> GetExpiredTempRolesAsync()
        {
            await using var context = new RiftContext();
            return await context.TempRoles
                .Where(x => x.ExpirationTime <= DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync()
        {
            await using var context = new RiftContext();
            return await context.TempRoles.CountAsync();
        }

        public async Task RemoveAsync(ulong userId, ulong roleId)
        {
            var rtr = new RiftTempRole
            {
                UserId = userId,
                RoleId = roleId,
            };

            await using var context = new RiftContext();
            context.TempRoles.Remove(rtr);
            await context.SaveChangesAsync();
        }

        public async Task<bool> HasAsync(ulong userId, ulong roleId)
        {
            await using var context = new RiftContext();
            return await context.TempRoles.AnyAsync(x => x.UserId == userId && x.RoleId == roleId);
        }
    }
}
