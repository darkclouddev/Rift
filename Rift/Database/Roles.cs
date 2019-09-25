using System.Collections.Generic;
using System.Threading.Tasks;

using Discord;

using Microsoft.EntityFrameworkCore;

using Rift.Data;
using Rift.Data.Models;

namespace Rift.Database
{
    public class Roles
    {
        public async Task AddAsync(IRole role)
        {
            await using var context = new RiftContext();
            var dbRole = new RiftRole
            {
                Name = role.Name,
                RoleId = role.Id
            };

            await context.Roles.AddAsync(dbRole);
            await context.SaveChangesAsync();
        }
        public async Task UpdateAsync(RiftRole role)
        {
            await using var context = new RiftContext();
            var entry = context.Entry(role);
            entry.Property(x => x.Name).IsModified = true;
            entry.Property(x => x.RoleId).IsModified = true;
                
            await context.SaveChangesAsync();
        }
        
        public async Task<RiftRole> GetAsync(int id)
        {
            await using var context = new RiftContext();
            return await context.Roles.FirstOrDefaultAsync(x => x.Id == id);
        }
        
        public async Task<RiftRole> GetByRoleIdAsync(ulong roleId)
        {
            await using var context = new RiftContext();
            return await context.Roles.FirstOrDefaultAsync(x => x.RoleId == roleId);
        }
        
        public async Task<List<RiftRole>> GetAllAsync()
        {
            await using var context = new RiftContext();
            return await context.Roles.ToListAsync();
        }
    }
}
