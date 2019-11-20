using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Rift.Data;
using Rift.Data.Models;

namespace Rift.Database
{
    public class ActiveEvents
    {
        public async Task<bool> AnyAsync(CancellationToken ct = default)
        {
            await using var context = new RiftContext();
            return await EntityFrameworkQueryableExtensions.AnyAsync(context.ActiveEvents, ct);
        }
        
        public async Task AddAsync(RiftActiveEvent activeEvent, CancellationToken ct = default)
        {
            await using var context = new RiftContext();
            await context.ActiveEvents.AddAsync(activeEvent, ct);
            await context.SaveChangesAsync(ct);
        }

        public async Task<RiftActiveEvent> GetClosestAsync(CancellationToken ct = default)
        {
            await using var context = new RiftContext();
            return await context.ActiveEvents
                .AsQueryable()
                .OrderBy(x => x.DueTime)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<List<RiftActiveEvent>> GetExpiredAsync(CancellationToken ct = default)
        {
            var dt = DateTime.UtcNow;

            await using var context = new RiftContext();
            return await context.ActiveEvents
                .AsQueryable()
                .Where(x => x.DueTime <= dt)
                .ToListAsync(ct);
        }
        
        public async Task<List<RiftActiveEvent>> GetAllAsync()
        {
            await using var context = new RiftContext();
            return await EntityFrameworkQueryableExtensions.ToListAsync(context.ActiveEvents);
        }

        public async Task RemoveAsync(int id)
        {
            var activeEvent = new RiftActiveEvent
            {
                Id = id
            };

            await using var context = new RiftContext();
            context.ActiveEvents.Remove(activeEvent);
            await context.SaveChangesAsync();
        }
    }
}
