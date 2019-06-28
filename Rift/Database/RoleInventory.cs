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
        public async Task<List<RiftRoleInventory>> GetAsync(ulong userId)
        {
            using (var context = new RiftContext())
            {
                return await context.RoleInventories
                    .Where(x => x.UserId == userId)
                    .ToListAsync();
            }
        }

        public async Task<int> GetCountAsync(ulong userId)
        {
            using (var context = new RiftContext())
            {
                return await context.RoleInventories
                    .CountAsync(x => x.UserId == userId);
            }
        }
    }
}
