using System;
using System.Threading.Tasks;

using Rift.Data;
using Rift.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace Rift.Database
{
    public class StoredMessages
    {
        public async Task AddAsync(RiftMessage message)
        {
            await using var context = new RiftContext();
            await context.Messages.AddAsync(message);
            await context.SaveChangesAsync();
        }

        public async Task<RiftMapping> GetMessageMappingByNameAsync(string identifier)
        {
            await using var context = new RiftContext();
            return await context.MessageMappings.FirstOrDefaultAsync(x =>
                x.Identifier.Equals(identifier, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task<RiftMessage> GetMessageByIdAsync(int id)
        {
            await using var context = new RiftContext();
            return await context.Messages.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
