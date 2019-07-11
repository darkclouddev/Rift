using System.Linq;
using System.Threading.Tasks;

using Rift.Data;
using Rift.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace Rift.Database
{
    public class ProfileBackgrounds
    {
        public async Task AddAsync(RiftProfileBackground background)
        {
            using (var context = new RiftContext())
            {
                await context.ProfileBackgrounds.AddAsync(background);
                await context.SaveChangesAsync();
            }
        }

        public async Task<RiftProfileBackground> GetAsync(int id)
        {
            using (var context = new RiftContext())
            {
                return await context.ProfileBackgrounds
                                    .FirstOrDefaultAsync(x => x.Id == id);
            }
        }

        public async Task<RiftProfileBackground[]> GetAllAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.ProfileBackgrounds
                                    .OrderBy(x => x.Id)
                                    .ToArrayAsync();
            }
        }
    }
}
