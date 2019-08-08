using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Rift.Data;
using Rift.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace Rift.Database
{
    public class EventSchedule
    {
        public async Task<bool> AnyAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.EventSchedule.AnyAsync();
            }
        }

        public async Task AddRangeAsync(IEnumerable<RiftScheduledEvent> eventList)
        {
            using (var context = new RiftContext())
            {
                await context.EventSchedule.AddRangeAsync(eventList);
                var affectedRows = await context.SaveChangesAsync();

                RiftBot.Log.Info($"Added {affectedRows.ToString()} event(s) to schedule.");
            }
        }

        public async Task<RiftScheduledEvent> GetClosestAsync(DateTime dt)
        {
            using (var context = new RiftContext())
            {
                return await context.EventSchedule
                                    .Where(x => x.StartAt > dt)
                                    .OrderBy(x => x.StartAt)
                                    .FirstOrDefaultAsync();
            }
        }
    }
}
