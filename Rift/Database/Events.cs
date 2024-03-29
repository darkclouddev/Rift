using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Rift.Data;
using Rift.Data.Models;
using Rift.Services;

namespace Rift.Database
{
    public class Events
    {
        public async Task<RiftEvent> GetAsync(string name)
        {
            await using var context = new RiftContext();
            return await context.Events
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Name.Equals(name, StringComparison.InvariantCulture));
        }

        public async Task<List<RiftEvent>> GetAllOfTypeAsync(EventType type)
        {
            var typeId = (int) type;

            return await GetAllOfTypeAsync(typeId);
        }

        public async Task<List<RiftEvent>> GetAllOfTypeAsync(int type)
        {
            await using var context = new RiftContext();
            return await context.Events
                .AsQueryable()
                .Where(x => x.Type == type)
                .ToListAsync();
        }
    }
}
