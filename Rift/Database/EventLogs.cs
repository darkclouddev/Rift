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
            await using var context = new RiftContext();
            await context.EventLogs.AddAsync(log);
            await context.SaveChangesAsync();
        }

        public async Task<RiftEventLog> GetAsync(int id)
        {
            await using var context = new RiftContext();
            return await context.EventLogs
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<RiftEventLog>> GetStartedAsync(Expression<Func<RiftEventLog, bool>> predicate)
        {
            await using var context = new RiftContext();
            return await context.EventLogs
                .Where(predicate)
                .ToListAsync();
        }
    }
}
