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
            using (var context = new RiftContext())
            {
                var dbRole = new RiftRole
                {
                    Name = role.Name,
                    RoleId = role.Id
                };

                await context.Roles.AddAsync(dbRole);
                await context.SaveChangesAsync();
            }
        }
        
        public async Task<RiftRole> GetAsync(int id)
        {
            using (var context = new RiftContext())
            {
                return await context.Roles.FirstOrDefaultAsync(x => x.Id == id);
            }
        }
        
        public async Task<List<RiftRole>> GetAllAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.Roles.ToListAsync();
            }
        }
    }
}
