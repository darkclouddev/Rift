﻿using System;
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
            await using var context = new RiftContext();
            return await context.EventSchedule
                .AsQueryable()
                .AnyAsync();
        }

        public async Task AddRangeAsync(IEnumerable<RiftScheduledEvent> eventList)
        {
            await using var context = new RiftContext();
            await context.EventSchedule.AddRangeAsync(eventList);
            var affectedRows = await context.SaveChangesAsync();

            RiftBot.Log.Information($"Added {affectedRows.ToString()} event(s) to schedule.");
        }

        public async Task<RiftScheduledEvent> GetClosestAsync(DateTime dt)
        {
            await using var context = new RiftContext();
            return await context.EventSchedule
                .AsQueryable()
                .Where(x => x.StartAt > dt)
                .OrderBy(x => x.StartAt)
                .FirstOrDefaultAsync();
        }
    }
}
