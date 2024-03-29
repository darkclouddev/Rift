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
            if (!await DB.Users.EnsureExistsAsync(userId))
                throw new DatabaseException(nameof(BackgroundInventory) + nameof(AddAsync));
            
            var backInv = new RiftBackgroundInventory
            {
                UserId = userId,
                BackgroundId = backgroundId
            };

            await using var context = new RiftContext();
            await context.BackgroundInventories.AddAsync(backInv);
            await context.SaveChangesAsync();
        }

        public async Task<List<RiftBackgroundInventory>> GetAsync(ulong userId)
        {
            await using var context = new RiftContext();
            return await context.BackgroundInventories
                .AsQueryable()
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.BackgroundId)
                .ToListAsync();
        }

        public async Task<List<ulong>> GetNitroBoostUsersAsync()
        {
            await using var context = new RiftContext();
            return await context.BackgroundInventories
                .AsQueryable()
                .Where(x => x.BackgroundId == 13) // nitro booster background
                .Select(x => x.UserId)
                .ToListAsync();
        }

        public async Task<int> CountAsync(ulong userId)
        {
            await using var context = new RiftContext();
            return await context.BackgroundInventories
                .AsQueryable()
                .Where(x => x.UserId == userId)
                .CountAsync();
        }

        public async Task<bool> HasAsync(ulong userId, int backgroundId)
        {
            await using var context = new RiftContext();
            return await context.BackgroundInventories
                .AsQueryable()
                .Where(x => x.UserId == userId && x.BackgroundId == backgroundId)
                .AnyAsync();
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
            await using var context = new RiftContext();
            if (await context.BackgroundInventories
                .AsQueryable()
                .AnyAsync(x => x.Equals(backInv)))
            {
                context.BackgroundInventories.Remove(backInv);
                await context.SaveChangesAsync();
            }
        }
        
        public async Task RemoveStreamersAsync(ulong userId)
        {
            var backs = await DB.Streamers.GetAllAsync();
            if (backs is null || backs.Count == 0)
                return;

            foreach (var inv in backs)
            {
                if (inv.BackgroundId == 0)
                    continue;
                
                await DB.BackgroundInventory.DeleteAsync(userId, inv.BackgroundId);
            }
        }

        public async Task UnsetStreamersBackgroundsAsync(ulong userId)
        {
            var streamers = await DB.Streamers.GetAllAsync();
            if (streamers is null || streamers.Count == 0)
                return;

            await UnsetBackgroundAsync(userId, streamers.Select(x => x.BackgroundId));
        }

        async Task UnsetBackgroundAsync(ulong userId, IEnumerable<int> items)
        {
            var dbUser = await DB.Users.GetAsync(userId);
            if (dbUser is null)
                return;

            if (dbUser.ProfileBackground == 0)
                return;

            if (items.All(x => x != dbUser.ProfileBackground))
                return;

            await DB.Users.SetBackgroundAsync(userId, 0);
        }
    }
}
