using System;
using System.Linq;
using System.Threading.Tasks;

using Rift.Data;
using Rift.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace Rift.Database
{
    public class Toxicity
    {
        static async Task<bool> EnsureExistsAsync(ulong userId)
        {
            if (!await DB.Users.EnsureExistsAsync(userId))
                throw new DatabaseException(nameof(EnsureExistsAsync));

            using (var context = new RiftContext())
            {
                if (await context.Toxicity.AnyAsync(x => x.UserId == userId))
                    return true;

                try
                {
                    var entry = new RiftToxicity
                    {
                        UserId = userId,
                        LastIncreased = DateTime.MinValue,
                    };

                    await context.Toxicity.AddAsync(entry);
                    await context.SaveChangesAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    RiftBot.Log.Error($"Failed to check {nameof(EnsureExistsAsync)} for user {userId.ToString()}.");
                    RiftBot.Log.Error(ex);
                    return false;
                }
            }
        }

        public async Task<RiftToxicity> GetAsync(ulong userId)
        {
            if (!await EnsureExistsAsync(userId))
                throw new DatabaseException(nameof(GetAsync));

            using (var context = new RiftContext())
            {
                return await context.Toxicity.FirstOrDefaultAsync(x => x.UserId == userId);
            }
        }

        public async Task<bool> HasBlockingAsync(ulong userId)
        {
            var toxicity = await GetAsync(userId);

            if (toxicity is null)
                return false;

            return toxicity.Level == 2u;
        }

        public async Task<RiftToxicity[]> GetNonZeroAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.Toxicity
                    .Where(x => x.Level > 0u)
                    .ToArrayAsync();
            }
        }

        public async Task UpdatePercentAsync(ulong userId, uint percent)
        {
            await EnsureExistsAsync(userId);

            var oldToxicity = await GetAsync(userId);

            using (var context = new RiftContext())
            {
                var toxicity = new RiftToxicity
                {
                    UserId = userId,
                    Percent = percent
                };

                context.Entry(toxicity).Property(x => x.Percent).IsModified = true;

                if (percent > oldToxicity.Percent)
                {
                    toxicity.LastIncreased = DateTime.UtcNow;
                    context.Entry(toxicity).Property(x => x.LastIncreased).IsModified = true;
                }
                else if (percent < oldToxicity.Percent)
                {
                    toxicity.LastDecreased = DateTime.UtcNow;
                    context.Entry(toxicity).Property(x => x.LastDecreased).IsModified = true;
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
