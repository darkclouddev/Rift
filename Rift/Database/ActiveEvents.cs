using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Rift.Data;
using Rift.Data.Models;

namespace Rift.Database
{
    public class ActiveEvents
    {
        public async Task<bool> AnyAsync()
        {
            await using var context = new RiftContext();
            return await context.ActiveEvents.AnyAsync();
        }
        
        public async Task AddAsync(RiftActiveEvent activeEvent)
        {
            await using var context = new RiftContext();
            await context.ActiveEvents.AddAsync(activeEvent);
            await context.SaveChangesAsync();
        }

        public async Task<RiftActiveEvent> GetClosestAsync()
        {
            await using var context = new RiftContext();
            return await context.ActiveEvents
                .OrderBy(x => x.DueTime)
                .FirstOrDefaultAsync();
        }

        public async Task<List<RiftActiveEvent>> GetExpiredAsync()
        {
            var dt = DateTime.UtcNow;

            await using var context = new RiftContext();
            return await context.ActiveEvents
                .Where(x => x.DueTime <= dt)
                .ToListAsync();
        }
        
        public async Task<List<RiftActiveEvent>> GetAllAsync()
        {
            await using var context = new RiftContext();
            return await context.ActiveEvents.ToListAsync();
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
