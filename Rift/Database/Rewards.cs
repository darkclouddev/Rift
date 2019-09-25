using System.Threading.Tasks;

using Rift.Data;
using Rift.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace Rift.Database
{
    public class Rewards
    {
        public async Task AddAsync(RiftReward reward)
        {
            await using var context = new RiftContext();
            await context.Rewards.AddAsync(reward);
            await context.SaveChangesAsync();
        }

        public async Task<RiftReward> GetAsync(int id)
        {
            await using var context = new RiftContext();
            return await context.Rewards
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> TryUpdateAsync(RiftReward reward)
        {
            await using var context = new RiftContext();
            var data = await GetAsync(reward.Id);

            if (data is null)
                return false; // nothing to update

            var entry = context.Entry(data);

            if (!reward.Description.Equals(data.Description))
                entry.Property(x => x.Description).IsModified = true;

            if (!reward.ItemsData.Equals(data.ItemsData))
                entry.Property(x => x.ItemsData).IsModified = true;

            if (!reward.RoleData.Equals(data.RoleData))
                entry.Property(x => x.RoleData).IsModified = true;

            await context.SaveChangesAsync();
            return true;
        }
    }
}
