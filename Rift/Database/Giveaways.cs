using System;
using System.Threading.Tasks;

using Rift.Data;
using Rift.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace Rift.Database
{
    public class Giveaways
    {
        public async Task AddOrUpdateAsync(RiftGiveaway giveaway)
        {
            using (var context = new RiftContext())
            {
                var dbGiveaway = await GetAsync(giveaway.Name);

                if (dbGiveaway is null)
                {
                    await context.Giveaways.AddAsync(giveaway);
                }
                else
                {
                    var entry = context.Entry(giveaway);

                    if (!dbGiveaway.Description.Equals(giveaway.Description))
                        entry.Property(x => x.Description).IsModified = true;

                    if (!dbGiveaway.WinnersAmount.Equals(giveaway.WinnersAmount))
                        entry.Property(x => x.WinnersAmount).IsModified = true;

                    if (!dbGiveaway.RewardId.Equals(giveaway.RewardId))
                        entry.Property(x => x.RewardId).IsModified = true;

                    if (!dbGiveaway.Duration.Equals(giveaway.Duration))
                        entry.Property(x => x.Duration).IsModified = true;

                    if (!dbGiveaway.CreatedAt.Equals(giveaway.CreatedAt))
                        entry.Property(x => x.CreatedAt).IsModified = true;

                    if (!dbGiveaway.CreatedBy.Equals(giveaway.CreatedBy))
                        entry.Property(x => x.CreatedBy).IsModified = true;
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task<RiftGiveaway> GetAsync(string name)
        {
            using (var context = new RiftContext())
            {
                return await context.Giveaways
                    .FirstOrDefaultAsync(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            }
        }
    }
}
