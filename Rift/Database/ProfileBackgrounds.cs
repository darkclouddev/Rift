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
            await using var context = new RiftContext();
            await context.Backgrounds.AddAsync(background);
            await context.SaveChangesAsync();
        }

        public async Task<RiftProfileBackground> GetAsync(int id)
        {
            await using var context = new RiftContext();
            return await context.Backgrounds.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<RiftProfileBackground[]> GetAllAsync()
        {
            await using var context = new RiftContext();
            return await context.Backgrounds
                .OrderBy(x => x.Id)
                .ToArrayAsync();
        }
    }
}
