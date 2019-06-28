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
            using (var context = new RiftContext())
            {
                return await context.Streamers.ToListAsync();
            }
        }

        public async Task<RiftStreamer> GetAsync(ulong userId)
        {
            using (var context = new RiftContext())
            {
                return await context.Streamers
                    .Where(x => x.UserId == userId)
                    .FirstOrDefaultAsync();
            }
        }
    }
}
