using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Rift.Data;
using Rift.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace Rift.Database
{
    public class Streamers
    {
        public async Task<List<RiftStreamer>> GetAllAsync()
        {
            await using var context = new RiftContext();
            return await context.Streamers
                .AsQueryable()
                .ToListAsync();
        }

        public async Task<RiftStreamer> GetAsync(ulong userId)
        {
            await using var context = new RiftContext();
            return await context.Streamers
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }
    }
}
