using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Rift.Data;
using Rift.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace Rift.Database
{
    public class ModerationLogs
    {
        public async Task AddAsync(ulong targetId, ulong moderatorId, string action, string reason, DateTime createdAt,
                                   TimeSpan duration)
        {
            var log = new RiftModerationLog
            {
                TargetId = targetId,
                ModeratorId = moderatorId,
                Action = action,
                Reason = reason,
                CreatedAt = createdAt,
                Duration = duration
            };

            await using var context = new RiftContext();
            await context.AddAsync(log);
            await context.SaveChangesAsync();
        }

        public async Task<List<RiftModerationLog>> GetAsync(ulong userId)
        {
            await using var context = new RiftContext();
            return await context.ModerationLog
                .Where(x => x.TargetId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Take(10)
                .ToListAsync();
        }

        public async Task<List<RiftModerationLog>> GetLastTenAsync()
        {
            await using var context = new RiftContext();
            return await context.ModerationLog
                .OrderByDescending(x => x.CreatedAt)
                .Take(10)
                .ToListAsync();
        }
    }
}
