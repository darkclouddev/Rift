using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Rift.Data;
using Rift.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace Rift.Database
{
    public class PendingUsers
    {
        public async Task AddAsync(RiftPendingUser pendingUser)
        {
            await DB.Users.EnsureExistsAsync(pendingUser.UserId);

            using (var context = new RiftContext())
            {
                await context.PendingUsers.AddAsync(pendingUser);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<RiftPendingUser>> GetAllAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.PendingUsers
                    .ToListAsync();
            }
        }

        public async Task<RiftPendingUser> GetAsync(ulong userId)
        {
            using (var context = new RiftContext())
            {
                return await context.PendingUsers
                    .Select(x => new RiftPendingUser
                    {
                        UserId = x.UserId,
                        Region = x.Region,
                        PlayerUUID = x.PlayerUUID,
                        AccountId = x.AccountId,
                        SummonedId = x.SummonedId,
                        ConfirmationCode = x.ConfirmationCode,
                        ExpirationTime = x.ExpirationTime
                    })
                    .Where(x => x.UserId == userId)
                    .FirstOrDefaultAsync();
            }
        }

        public async Task RemoveAsync(RiftPendingUser pendingUser)
        {
            await RemoveAsync(pendingUser.UserId);
        }

        public async Task RemoveAsync(ulong userId)
        {
            using (var context = new RiftContext())
            {
                var pendingUser = new RiftPendingUser
                {
                    UserId = userId
                };

                context.PendingUsers.Remove(pendingUser);
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsPendingAsync(ulong userId)
        {
            using (var context = new RiftContext())
            {
                return await context.PendingUsers.AnyAsync(x => x.UserId == userId);
            }
        }

        public async Task<List<RiftPendingUser>> GetExpiredAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.PendingUsers
                    .Where(x => x.ExpirationTime > DateTime.UtcNow)
                    .ToListAsync();
            }
        }
    }
}
