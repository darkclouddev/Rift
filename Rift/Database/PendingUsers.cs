﻿using System;
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

            await using var context = new RiftContext();
            await context.PendingUsers.AddAsync(pendingUser);
            await context.SaveChangesAsync();
        }

        public async Task<List<RiftPendingUser>> GetAllAsync()
        {
            await using var context = new RiftContext();
            return await context.PendingUsers
                .AsQueryable()
                .ToListAsync();
        }

        public async Task<RiftPendingUser> GetAsync(ulong userId)
        {
            await using var context = new RiftContext();
            return await context.PendingUsers
                .AsQueryable()
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

        public async Task RemoveAsync(RiftPendingUser pendingUser)
        {
            await RemoveAsync(pendingUser.UserId);
        }

        public async Task RemoveAsync(ulong userId)
        {
            await using var context = new RiftContext();
            var pendingUser = new RiftPendingUser
            {
                UserId = userId
            };

            context.PendingUsers.Remove(pendingUser);
            await context.SaveChangesAsync();
        }

        public async Task<bool> IsPendingAsync(ulong userId)
        {
            await using var context = new RiftContext();
            return await context.PendingUsers
                .AsQueryable()
                .AnyAsync(x => x.UserId == userId);
        }

        public async Task<List<RiftPendingUser>> GetExpiredAsync()
        {
            await using var context = new RiftContext();
            return await context.PendingUsers
                .AsQueryable()
                .Where(x => x.ExpirationTime > DateTime.UtcNow)
                .ToListAsync();
        }
    }
}
