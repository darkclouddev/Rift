using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Rift.Data;
using Rift.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace Rift.Database
{
    public class GiveawayLogs
    {
        public async Task AddAsync(RiftGiveawayLog log)
        {
            using (var context = new RiftContext())
            {
                await context.GiveawayLogs.AddAsync(log);
                await context.SaveChangesAsync();
            }
        }

        public async Task<RiftGiveawayLog> GetAsync(int id)
        {
            using (var context = new RiftContext())
            {
                return await context.GiveawayLogs.FirstOrDefaultAsync(x => x.Id == id);
            }
        }

        public async Task<List<RiftGiveawayLog>> GetStartedAsync(Expression<Func<RiftGiveawayLog, bool>> predicate)
        {
            using (var context = new RiftContext())
            {
                return await context.GiveawayLogs
                                    .Where(predicate)
                                    .ToListAsync();
            }
        }
    }
}
