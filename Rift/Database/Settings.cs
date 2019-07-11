using System.Threading.Tasks;

using Rift.Data;
using Rift.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace Rift.Database
{
    public class Settings
    {
        public async Task<RiftSettings> GetAsync(int id)
        {
            using (var context = new RiftContext())
            {
                return await context.Settings
                                    .FirstOrDefaultAsync(x => x.Id == id);
            }
        }

        public async Task SetAsync(int id, string data)
        {
            using (var context = new RiftContext())
            {
                var settings = new RiftSettings
                {
                    Id = id,
                    Data = data
                };

                if (await context.Settings.AnyAsync(x => x.Id == id))
                    context.Entry(settings).Property(x => x.Data).IsModified = true;
                else
                    await context.Settings.AddAsync(settings);

                await context.SaveChangesAsync();
            }
        }
    }
}
