using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Rift.Data;
using Rift.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace Rift.Database
{
    public class RoleInventory
    {
        public async Task AddAsync(ulong userId, int roleId, string source)
        {
            var invRole = new RiftRoleInventory
            {
                UserId = userId,
                RoleId = roleId,
                ObtainedAt = DateTime.UtcNow,
                ObtainedFrom = source
            };

            await using var context = new RiftContext();
            await context.RoleInventories.AddAsync(invRole);
            await context.SaveChangesAsync();
        }

        public async Task<List<RiftRoleInventory>> GetAsync(ulong userId)
        {
            await using var context = new RiftContext();
            return await context.RoleInventories
                .AsQueryable()
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<int> CountAsync(ulong userId)
        {
            await using var context = new RiftContext();
            return await context.RoleInventories
                .AsQueryable()
                .CountAsync(x => x.UserId == userId);
        }
        
        public async Task<bool> HasAnyAsync(ulong userId, params int[] roleIds)
        {
            await using var context = new RiftContext();
            return await context.RoleInventories
                .AsQueryable()
                .AnyAsync(x => x.UserId == userId
                               && roleIds.Contains(x.RoleId));
        }
        
        public async Task<bool> HasAllAsync(ulong userId, params int[] roleIds)
        {
            await using var context = new RiftContext();
            return await context.RoleInventories
                .AsQueryable()
                .AllAsync(x => x.UserId == userId
                               && roleIds.Contains(x.RoleId));
        }
    }
}
