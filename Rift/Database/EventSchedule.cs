using System.Collections.Generic;
using System.Threading.Tasks;

using Rift.Data;

using Microsoft.EntityFrameworkCore;

using Rift.Data.Models;

namespace Rift.Database
{
    public class EventSchedule
    {
        public async Task<bool> AnyAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.EventSchedule
                                    .AnyAsync();
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
    }
}
