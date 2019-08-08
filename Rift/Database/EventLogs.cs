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
    public class EventLogs
    {
        public async Task AddAsync(RiftEventLog log)
        {
            using (var context = new RiftContext())
            {
                await context.EventLogs.AddAsync(log);
                await context.SaveChangesAsync();
            }
        }

        public async Task<RiftEventLog> GetAsync(int id)
        {
            using (var context = new RiftContext())
            {
                return await context.EventLogs.FirstOrDefaultAsync(x => x.Id == id);
            }
        }

        public async Task<List<RiftEventLog>> GetStartedAsync(Expression<Func<RiftEventLog, bool>> predicate)
        {
            using (var context = new RiftContext())
            {
                return await context.EventLogs
                                    .Where(predicate)
                                    .ToListAsync();
            }
        }
    }
}
