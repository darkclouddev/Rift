using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Rift.Data;
using Rift.Data.Models;
using Rift.Events;

using Microsoft.EntityFrameworkCore;

namespace Rift.Database
{
    public class Inventory
    {
        public EventHandler<CoinsReceivedEventArgs> OnCoinsReceived;
        public EventHandler<CoinsSpentEventArgs> OnCoinsSpent;

        static async Task<bool> EnsureExistsAsync(ulong userId)
        {
            if (!await DB.Users.EnsureExistsAsync(userId))
                return false;

            using (var context = new RiftContext())
            {
                if (await context.Inventory.AnyAsync(x => x.UserId == userId))
                    return true;

                try
                {
                    var entry = new RiftInventory
                    {
                        UserId = userId,
                    };

                    await context.Inventory.AddAsync(entry);
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

        public async Task AddAsync(ulong userId, InventoryData data)
        {
            var dbInventory = await GetAsync(userId);
            var inventory = new RiftInventory {UserId = userId};
            var stat = new StatisticData();

            using (var context = new RiftContext())
            {
                var inventoryEntry = context.Attach(inventory);

                if (data.Coins.HasValue)
                {
                    var coinsBefore = dbInventory.Coins;
                    var value = data.Coins.Value;

                    OnCoinsReceived?.Invoke(null, new CoinsReceivedEventArgs(userId, value));

                    if (uint.MaxValue - coinsBefore < value)
                        inventory.Coins = uint.MaxValue;
                    else
                        inventory.Coins = coinsBefore + value;

                    stat.CoinsEarned = inventory.Coins - coinsBefore;

                    inventoryEntry.Property(x => x.Coins).IsModified = true;
                    RiftBot.Log.Info(
                        $"Modified {userId.ToString()}'s coin(s): ({coinsBefore.ToString()} => {inventory.Coins.ToString()})");
                }

                if (data.Tokens.HasValue)
                {
                    var tokensBefore = dbInventory.Tokens;
                    var value = data.Tokens.Value;

                    if (uint.MaxValue - tokensBefore < value)
                        inventory.Tokens = uint.MaxValue;
                    else
                        inventory.Tokens = tokensBefore + value;

                    stat.TokensEarned = inventory.Tokens - tokensBefore;

                    inventoryEntry.Property(x => x.Tokens).IsModified = true;
                    RiftBot.Log.Info(
                        $"Modified {userId.ToString()}'s token(s): ({tokensBefore.ToString()} => {inventory.Tokens.ToString()})");
                }

                if (data.Essence.HasValue)
                {
                    var essenceBefore = dbInventory.Essence;
                    var value = data.Essence.Value;

                    if (uint.MaxValue - essenceBefore < value)
                        inventory.Essence = uint.MaxValue;
                    else
                        inventory.Essence = essenceBefore + value;

                    stat.EssenceEarned = inventory.Essence - essenceBefore;

                    inventoryEntry.Property(x => x.Essence).IsModified = true;
                    RiftBot.Log.Info(
                        $"Modified {userId.ToString()}'s essence: ({essenceBefore.ToString()} => {inventory.Essence.ToString()})");
                }

                if (data.Chests.HasValue)
                {
                    var chestsBefore = dbInventory.Chests;
                    var value = data.Chests.Value;

                    if (uint.MaxValue - chestsBefore < value)
                        inventory.Chests = uint.MaxValue;
                    else
                        inventory.Chests = chestsBefore + value;

                    stat.ChestsEarned = inventory.Chests - chestsBefore;

                    inventoryEntry.Property(x => x.Chests).IsModified = true;
                    RiftBot.Log.Info(
                        $"Modified {userId.ToString()}'s chest(s): ({chestsBefore.ToString()} => {inventory.Chests.ToString()})");
                }

                if (data.Spheres.HasValue)
                {
                    var spheresBefore = dbInventory.Spheres;
                    var value = data.Spheres.Value;

                    if (uint.MaxValue - spheresBefore < value)
                        inventory.Spheres = uint.MaxValue;
                    else
                        inventory.Spheres = spheresBefore + value;

                    stat.SpheresEarned = inventory.Spheres - spheresBefore;

                    inventoryEntry.Property(x => x.Spheres).IsModified = true;
                    RiftBot.Log.Info(
                        $"Modified {userId.ToString()}'s sphere(s): ({spheresBefore.ToString()} => {inventory.Spheres.ToString()})");
                }

                if (data.Capsules.HasValue)
                {
                    var capsulesBefore = dbInventory.Capsules;
                    var value = data.Capsules.Value;

                    if (uint.MaxValue - capsulesBefore < value)
                        inventory.Capsules = uint.MaxValue;
                    else
                        inventory.Capsules = capsulesBefore + value;

                    stat.CapsulesEarned = inventory.Capsules - capsulesBefore;

                    inventoryEntry.Property(x => x.Capsules).IsModified = true;
                    RiftBot.Log.Info(
                        $"Modified {userId.ToString()}'s capsule(s): ({capsulesBefore.ToString()} => {inventory.Capsules.ToString()})");
                }

                if (data.Tickets.HasValue)
                {
                    var ticketsBefore = dbInventory.Tickets;
                    var value = data.Tickets.Value;

                    if (uint.MaxValue - ticketsBefore < value)
                        inventory.Tickets = uint.MaxValue;
                    else
                        inventory.Tickets = ticketsBefore + value;

                    stat.TicketsEarned = inventory.Tickets - ticketsBefore;

                    inventoryEntry.Property(x => x.Tickets).IsModified = true;
                    RiftBot.Log.Info(
                        $"Modified {userId.ToString()}'s ticket(s): ({ticketsBefore.ToString()} => {inventory.Tickets.ToString()})");
                }

                if (data.DoubleExps.HasValue)
                {
                    var doubleExpsBefore = dbInventory.BonusDoubleExp;
                    var value = data.DoubleExps.Value;

                    if (uint.MaxValue - doubleExpsBefore < value)
                        inventory.BonusDoubleExp = uint.MaxValue;
                    else
                        inventory.BonusDoubleExp = doubleExpsBefore + value;

                    stat.DoubleExpsEarned = inventory.BonusDoubleExp - doubleExpsBefore;

                    inventoryEntry.Property(x => x.BonusDoubleExp).IsModified = true;
                    RiftBot.Log.Info(
                        $"Modified {userId.ToString()}'s doubleExp(s): ({doubleExpsBefore.ToString()} => {inventory.BonusDoubleExp.ToString()})");
                }

                if (data.BotRespects.HasValue)
                {
                    var respectsBefore = dbInventory.BonusBotRespect;
                    var value = data.BotRespects.Value;

                    if (uint.MaxValue - respectsBefore < value)
                        inventory.BonusBotRespect = uint.MaxValue;
                    else
                        inventory.BonusBotRespect = respectsBefore + value;

                    stat.BotRespectsEarned = inventory.BonusBotRespect - respectsBefore;

                    inventoryEntry.Property(x => x.BonusBotRespect).IsModified = true;
                    RiftBot.Log.Info(
                        $"Modified {userId.ToString()}'s respect(s): ({respectsBefore.ToString()} => {inventory.BonusBotRespect.ToString()})");
                }

                if (data.Rewinds.HasValue)
                {
                    var rewindsBefore = dbInventory.BonusRewind;
                    var value = data.Rewinds.Value;

                    if (uint.MaxValue - rewindsBefore < value)
                        inventory.BonusRewind = uint.MaxValue;
                    else
                        inventory.BonusRewind = rewindsBefore + value;

                    stat.RewindsEarned = inventory.BonusRewind - rewindsBefore;

                    inventoryEntry.Property(x => x.BonusRewind).IsModified = true;
                    RiftBot.Log.Info(
                        $"Modified {userId.ToString()}'s rewind(s): ({rewindsBefore.ToString()} => {inventory.BonusRewind.ToString()})");
                }

                await context.SaveChangesAsync();
                await DB.Statistics.AddAsync(userId, stat);
            }
        }

        public async Task<RiftInventory> GetAsync(ulong userId)
        {
            if (!await EnsureExistsAsync(userId)
                || !await DB.Statistics.EnsureExistsAsync(userId))
                throw new DatabaseException(nameof(GetAsync));

            using (var context = new RiftContext())
            {
                return await context.Inventory
                                    .FirstOrDefaultAsync(x => x.UserId == userId);
            }
        }

        public async Task<List<RiftInventory>> GetAllAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.Inventory
                                    .ToListAsync();
            }
        }

        public async Task<List<RiftInventory>> GetAsync(Expression<Func<RiftInventory, bool>> predicate)
        {
            using (var context = new RiftContext())
            {
                return await context.Inventory
                                    .Where(predicate)
                                    .ToListAsync();
            }
        }

        public async Task<int> GiveTicketsToUsersAsync()
        {
            using (var context = new RiftContext())
            {
                return await context.Database.ExecuteSqlCommandAsync(
                    "CALL giveticketstolowlevels();"); // MariaDB-specific call.
            }
        }

        public async Task RemoveAsync(ulong userId, InventoryData data)
        {
            var dbInventory = await GetAsync(userId);
            var dbStatistics = await DB.Statistics.GetAsync(userId);

            var inventory = new RiftInventory {UserId = userId};
            var stat = new StatisticData();

            using (var context = new RiftContext())
            {
                var inventoryEntry = context.Attach(inventory);

                if (data.Coins.HasValue)
                {
                    var coinsBefore = dbInventory.Coins;
                    var value = Math.Min(data.Coins.Value, coinsBefore);

                    if (value > uint.MinValue)
                    {
                        OnCoinsSpent?.Invoke(null, new CoinsSpentEventArgs(userId, value));

                        inventory.Coins = coinsBefore - value;

                        if (uint.MaxValue - dbStatistics.CoinsSpent < value)
                            stat.CoinsSpent = uint.MaxValue;
                        else
                            stat.CoinsSpent = dbStatistics.CoinsSpent + value;

                        inventoryEntry.Property(x => x.Coins).IsModified = true;
                        RiftBot.Log.Info(
                            $"Modified {userId.ToString()}'s coin(s): ({coinsBefore.ToString()} => {inventory.Coins.ToString()})");
                    }
                }

                if (data.Tokens.HasValue)
                {
                    var tokensBefore = dbInventory.Tokens;
                    var value = Math.Min(data.Tokens.Value, tokensBefore);

                    if (value > uint.MinValue)
                    {
                        inventory.Tokens = tokensBefore - value;

                        if (uint.MaxValue - dbStatistics.TokensSpent < value)
                            stat.TokensSpent = uint.MaxValue;
                        else
                            stat.TokensSpent = dbStatistics.TokensSpent + value;

                        inventoryEntry.Property(x => x.Tokens).IsModified = true;
                        RiftBot.Log.Info(
                            $"Modified {userId.ToString()}'s token(s): ({tokensBefore.ToString()} => {inventory.Tokens.ToString()})");
                    }
                }

                if (data.Essence.HasValue)
                {
                    var essenceBefore = dbInventory.Essence;
                    var value = Math.Min(data.Essence.Value, essenceBefore);

                    if (value > uint.MinValue)
                    {
                        inventory.Chests = essenceBefore - value;

                        if (uint.MaxValue - dbStatistics.ChestsOpened < value)
                            stat.ChestsOpened = uint.MaxValue;
                        else
                            stat.ChestsOpened = dbStatistics.ChestsOpened + value;

                        inventoryEntry.Property(x => x.Essence).IsModified = true;
                        RiftBot.Log.Info(
                            $"Modified {userId.ToString()}'s essence: ({essenceBefore.ToString()} => {inventory.Essence.ToString()})");
                    }
                }

                if (data.Chests.HasValue)
                {
                    var chestsBefore = dbInventory.Chests;
                    var value = Math.Min(data.Chests.Value, chestsBefore);

                    if (value > uint.MinValue)
                    {
                        inventory.Chests = chestsBefore - value;

                        if (uint.MaxValue - dbStatistics.ChestsOpened < value)
                            stat.ChestsOpened = uint.MaxValue;
                        else
                            stat.ChestsOpened = dbStatistics.ChestsOpened + value;

                        inventoryEntry.Property(x => x.Chests).IsModified = true;
                        RiftBot.Log.Info(
                            $"Modified {userId.ToString()}'s chest(s): ({chestsBefore.ToString()} => {inventory.Chests.ToString()})");
                    }
                }

                if (data.Spheres.HasValue)
                {
                    var spheresBefore = dbInventory.Spheres;
                    var value = Math.Min(data.Spheres.Value, spheresBefore);

                    if (value > uint.MinValue)
                    {
                        inventory.Spheres = spheresBefore - value;

                        if (uint.MaxValue - dbStatistics.SpheresOpened < value)
                            stat.SpheresOpened = uint.MaxValue;
                        else
                            stat.SpheresOpened = dbStatistics.SpheresOpened + value;

                        inventoryEntry.Property(x => x.Spheres).IsModified = true;
                        RiftBot.Log.Info(
                            $"Modified {userId.ToString()}'s sphere(s): ({spheresBefore.ToString()} => {inventory.Spheres.ToString()})");
                    }
                }

                if (data.Capsules.HasValue)
                {
                    var capsulesBefore = dbInventory.Capsules;
                    var value = Math.Min(data.Capsules.Value, capsulesBefore);

                    if (value > uint.MinValue)
                    {
                        inventory.Capsules = capsulesBefore - value;

                        if (uint.MaxValue - dbStatistics.CapsulesOpened < value)
                            stat.CapsulesOpened = uint.MaxValue;
                        else
                            stat.CapsulesOpened = dbStatistics.CapsulesOpened + value;

                        inventoryEntry.Property(x => x.Capsules).IsModified = true;
                        RiftBot.Log.Info(
                            $"Modified {userId.ToString()}'s capsule(s): ({capsulesBefore.ToString()} => {inventory.Capsules.ToString()})");
                    }
                }

                if (data.Tickets.HasValue)
                {
                    var ticketsBefore = dbInventory.Tickets;
                    var value = Math.Min(data.Tickets.Value, ticketsBefore);

                    if (value > uint.MinValue)
                    {
                        inventory.Tickets = ticketsBefore - value;

                        if (uint.MaxValue - dbStatistics.TicketsSpent < value)
                            stat.TicketsSpent = uint.MaxValue;
                        else
                            stat.TicketsSpent = dbStatistics.TicketsSpent + value;

                        inventoryEntry.Property(x => x.Tickets).IsModified = true;
                        RiftBot.Log.Info(
                            $"Modified {userId.ToString()}'s usual ticket(s): ({ticketsBefore.ToString()} => {inventory.Tickets.ToString()})");
                    }
                }

                if (data.DoubleExps.HasValue)
                {
                    var doubleExpsBefore = dbInventory.BonusDoubleExp;
                    var value = Math.Min(data.DoubleExps.Value, doubleExpsBefore);

                    if (value > uint.MinValue)
                    {
                        inventory.BonusDoubleExp = doubleExpsBefore - value;

                        if (uint.MaxValue - dbStatistics.DoubleExpsActivated < value)
                            stat.DoubleExpsActivated = uint.MaxValue;
                        else
                            stat.DoubleExpsActivated = dbStatistics.DoubleExpsActivated + value;

                        inventoryEntry.Property(x => x.BonusDoubleExp).IsModified = true;
                        RiftBot.Log.Info(
                            $"Modified {userId.ToString()}'s doubleExp(s): ({doubleExpsBefore.ToString()} => {inventory.BonusDoubleExp.ToString()})");
                    }
                }

                if (data.BotRespects.HasValue)
                {
                    var respectsBefore = dbInventory.BonusBotRespect;
                    var value = Math.Min(data.BotRespects.Value, respectsBefore);

                    if (value > uint.MinValue)
                    {
                        inventory.BonusBotRespect = respectsBefore - value;

                        if (uint.MaxValue - dbStatistics.BotRespectsActivated < value)
                            stat.BotRespectsActivated = uint.MaxValue;
                        else
                            stat.BotRespectsActivated = dbStatistics.BotRespectsActivated + value;

                        inventoryEntry.Property(x => x.BonusBotRespect).IsModified = true;
                        RiftBot.Log.Info(
                            $"Modified {userId.ToString()}'s respect(s): ({respectsBefore.ToString()} => {inventory.BonusBotRespect.ToString()})");
                    }
                }

                if (data.Rewinds.HasValue)
                {
                    var rewindsBefore = dbInventory.BonusRewind;
                    var value = Math.Min(data.Rewinds.Value, rewindsBefore);

                    if (value > uint.MinValue)
                    {
                        inventory.BonusRewind = rewindsBefore - value;

                        if (uint.MaxValue - dbStatistics.RewindsActivated < value)
                            stat.RewindsActivated = uint.MaxValue;
                        else
                            stat.RewindsActivated = dbStatistics.RewindsActivated + value;

                        inventoryEntry.Property(x => x.BonusRewind).IsModified = true;
                        RiftBot.Log.Info(
                            $"Modified {userId.ToString()}'s rewind(s): ({rewindsBefore.ToString()} => {inventory.BonusRewind.ToString()})");
                    }
                }

                await context.SaveChangesAsync();
            }
        }
    }

    public class InventoryData
    {
        public uint? Coins { get; set; }
        public uint? Tokens { get; set; }
        public uint? Essence { get; set; }
        public uint? Chests { get; set; }
        public uint? Spheres { get; set; }
        public uint? Capsules { get; set; }
        public uint? Tickets { get; set; }
        public uint? DoubleExps { get; set; }
        public uint? BotRespects { get; set; }
        public uint? Rewinds { get; set; }
    }
}
