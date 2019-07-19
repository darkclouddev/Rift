using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Rift.Data;
using Rift.Data.Models;

namespace Rift.Database
{
    public class Teams
    {
        public async Task<RiftTeam> GetAsync(int id)
        {
            using (var context = new RiftContext())
            {
                return await context.Teams
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
        }
    }
}
