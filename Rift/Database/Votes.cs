using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Rift.Data;
using Rift.Data.Models;

namespace Rift.Database
{
    public class Votes
    {
        public async Task AddOrUpdateAsync(ulong userId, int communityId = 0, int teamId = 0, ulong streamerId = 0ul)
        {
            var votes = new RiftVote
            {
                UserId = userId,
                CommunityId = communityId,
                TeamId = teamId,
                StreamerId = streamerId
            };

            using (var context = new RiftContext())
            {
                if (!await context.Votes.AnyAsync(x => x.UserId == userId))
                {
                    await context.Votes.AddAsync(votes);
                    await context.SaveChangesAsync();
                    return;
                }

                var entry = context.Entry(votes);

                if (communityId != 0)
                    entry.Property(x => x.CommunityId).IsModified = true;

                if (teamId != 0)
                    entry.Property(x => x.TeamId).IsModified = true;

                if (streamerId != 0)
                    entry.Property(x => x.StreamerId).IsModified = true;

                await context.SaveChangesAsync();
            }
        }

        public async Task<RiftVote> GetAsync(ulong userId)
        {
            using (var context = new RiftContext())
            {
                return await context.Votes
                    .FirstOrDefaultAsync(x => x.UserId == userId);
            }
        }
    }
}
