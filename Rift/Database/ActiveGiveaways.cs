using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Rift.Data;
using Rift.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace Rift.Database
{
    public class ActiveGiveaways
    {
        public async Task AddAsync(RiftGiveawayActive giveaway)
        {
            using (var context = new RiftContext())
            {
                await context.ActiveGiveaways.AddAsync(giveaway);
                await context.SaveChangesAsync();
            }
        }

        public async Task<RiftGiveawayActive> GetClosestAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.ActiveGiveaways
                    .OrderBy(x => x.DueTime)
                    .FirstOrDefaultAsync();
            }
        }

        public async Task<List<RiftGiveawayActive>> GetExpiredAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.ActiveGiveaways
                    .Where(x => x.DueTime < DateTime.UtcNow)
                    .ToListAsync();
            }
        }

        public async Task<List<RiftGiveawayActive>> GetLinkedAsync(string giveawayName)
        {
            using (var context = new RiftContext())
            {
                return await context.ActiveGiveaways
                    .Where(x => x.GiveawayName.Equals(giveawayName))
                    .ToListAsync();
            }
        }

        public async Task<RiftGiveawayActive> GetAsync(int id)
        {
            using (var context = new RiftContext())
            {
                return await context.ActiveGiveaways
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
        }

        public async Task<List<RiftGiveawayActive>> GetAllAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.ActiveGiveaways.ToListAsync();
            }
        }

        public async Task RemoveAsync(int id)
        {
            var giveaway = new RiftGiveawayActive
            {
                Id = id
            };
            
            using (var context = new RiftContext())
            {
                context.ActiveGiveaways.Remove(giveaway);
                await context.SaveChangesAsync();
            }
        }
    }
}
