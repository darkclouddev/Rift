using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
                                    .OrderBy(x => x.BackgroundId)
                                    .ToListAsync();
            }
        }

        public async Task<List<RiftBackgroundInventory>> GetNitroBoostUsersAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.BackgroundInventories
                                    .Where(x => x.BackgroundId == 13) // nitro booster background
                                    .ToListAsync();
            }
        }

        public async Task<int> CountAsync(ulong userId)
        {
            using (var context = new RiftContext())
            {
                return await context.BackgroundInventories
                                    .Where(x => x.UserId == userId)
                                    .CountAsync();
            }
        }

        public async Task<bool> HasAsync(ulong userId, int backgroundId)
        {
            using (var context = new RiftContext())
            {
                return await context.BackgroundInventories
                                    .Where(x => x.UserId == userId && x.BackgroundId == backgroundId)
                                    .AnyAsync();
            }
        }

        public async Task DeleteAsync(ulong userId, int backgroundId)
        {
            var backInv = new RiftBackgroundInventory
            {
                UserId = userId,
                BackgroundId = backgroundId
            };

            await DeleteAsync(backInv);
        }

        public async Task DeleteAsync(RiftBackgroundInventory backInv)
        {
            using (var context = new RiftContext())
            {
                context.BackgroundInventories.Remove(backInv);
                await context.SaveChangesAsync();
            }
        }
    }
}
