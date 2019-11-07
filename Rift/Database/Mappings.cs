using System;
using System.Threading.Tasks;

using Rift.Data;
using Rift.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace Rift.Database
{
    public class Mappings
    {
        public async Task<RiftMapping> GetByNameAsync(string identifier)
        {
            await using var context = new RiftContext();
            return await context.Mappings.FirstOrDefaultAsync(x =>
                x.Id.Equals(identifier, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
