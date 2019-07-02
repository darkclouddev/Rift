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
    public class ScheduledEvents
    {
        public async Task<List<RiftScheduledEvent>> GetAllEventsAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.ScheduledEvents
                    .OrderBy(x => x.Date)
                    .ToListAsync();
            }
        }
        
        public async Task<List<RiftGiveawayActive>> GetActiveEventsAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.ActiveGiveaways
                    .OrderBy(x => x.DueTime)
                    .ToListAsync();
            }
        }

        public async Task<List<RiftScheduledEvent>> GetEventsAsync(Expression<Func<RiftScheduledEvent, bool>> predicate)
        {
            using (var context = new RiftContext())
            {
                return await context.ScheduledEvents
                    .Where(predicate)
                    .OrderBy(x => x.Date)
                    .ToListAsync();
            }
        }

        public async Task<RiftScheduledEvent> GetClosestEventAsync(DateTime dt)
        {
            using (var context = new RiftContext())
            {
                return await context.ScheduledEvents
                    .Where(x => x.Date >= dt)
                    .OrderBy(x => x.Date)
                    .FirstOrDefaultAsync();
            }
        }
    }
}
