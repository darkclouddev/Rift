using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Rift.Data;
using Rift.Data.Models;

namespace Rift.Database
{
    public class BackgroundInventory
    {
        public async Task AddAsync(ulong userId, int backgroundId)
        {
            var backInv = new RiftBackgroundInventory
            {
                UserId = userId,
                BackgroundId = backgroundId
            };

            using (var context = new RiftContext())
            {
                await context.BackgroundInventories.AddAsync(backInv);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<RiftBackgroundInventory>> GetAsync(ulong userId)
        {
            using (var context = new RiftContext())
            {
                return await context.BackgroundInventories
                                    .Where(x => x.UserId == userId)
                                    .ToListAsync();
            }
        }

        public async Task SetAsBackgroundAsync(ulong userId, int backgroundId)
        {
            var dbUser = new RiftUser
            {
                UserId = userId,
                ProfileBackground = backgroundId
            };

            using (var context = new RiftContext())
            {
                context.Entry(dbUser).Property(x => x.ProfileBackground).IsModified = true;
                await context.SaveChangesAsync();
            }
        }
    }
}
