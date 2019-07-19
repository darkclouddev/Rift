using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Rift.Data;
using Rift.Data.Models;

namespace Rift.Database
{
    public class Communities
    {
        public async Task<RiftCommunity> GetAsync(int id)
        {
            using (var context = new RiftContext())
            {
                return await context.Communities
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
        }
    }
}
