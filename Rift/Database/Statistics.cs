using System;
using System.Threading.Tasks;

using Rift.Data;
using Rift.Data.Models;
using Rift.Events;

using Microsoft.EntityFrameworkCore;

namespace Rift.Database
{
    public class Statistics
    {
        public event EventHandler<MessageCreatedEventArgs> OnMessageCreated;
        public event EventHandler<VoiceUptimeEarnedEventArgs> VoiceUptimeEarned;

        public async Task<bool> EnsureExistsAsync(ulong userId)
        {
            if (!await DB.Users.EnsureExistsAsync(userId))
                return false;

            using (var context = new RiftContext())
            {
                if (await context.Statistics.AnyAsync(x => x.UserId == userId))
                    return true;

                try
                {
                    var entry = new RiftStatistics
                    {
                        UserId = userId,
                    };

                    await context.Statistics.AddAsync(entry);
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

        public async Task AddAsync(ulong userId, StatisticData data)
        {
            if (!await EnsureExistsAsync(userId))
                throw new DatabaseException(nameof(AddAsync));

            var dbStatistics = await GetAsync(userId);

            var stat = new RiftStatistics
            {
                UserId = userId
            };

            using (var context = new RiftContext())
            {
                var entry = context.Attach(stat);

                if (data.CoinsEarned.HasValue)
                {
                    var before = dbStatistics.CoinsEarned;
                    var value = data.CoinsEarned.Value;

                    if (uint.MaxValue - before < value)
                        stat.CoinsEarned = uint.MaxValue;
                    else
                        stat.CoinsEarned = before + value;

                    entry.Property(x => x.CoinsEarned).IsModified = true;
                }

                if (data.CoinsSpent.HasValue)
                {
                    var before = dbStatistics.CoinsSpent;
                    var value = data.CoinsSpent.Value;

                    if (uint.MaxValue - before < value)
                        stat.CoinsSpent = uint.MaxValue;
                    else
                        stat.CoinsSpent = before + value;

                    entry.Property(x => x.CoinsSpent).IsModified = true;
                }

                if (data.TokensEarned.HasValue)
                {
                    var before = dbStatistics.TokensEarned;
                    var value = data.TokensEarned.Value;

                    if (uint.MaxValue - before < value)
                        stat.TokensEarned = uint.MaxValue;
                    else
                        stat.TokensEarned = before + value;

                    entry.Property(x => x.TokensEarned).IsModified = true;
                }

                if (data.TokensSpent.HasValue)
                {
                    var before = dbStatistics.TokensSpent;
                    var value = data.TokensSpent.Value;

                    if (uint.MaxValue - before < value)
                        stat.TokensSpent = uint.MaxValue;
                    else
                        stat.TokensSpent = before + value;

                    entry.Property(x => x.TokensSpent).IsModified = true;
                }

                if (data.EssenceEarned.HasValue)
                {
                    var before = dbStatistics.EssenceEarned;
                    var value = data.EssenceEarned.Value;

                    if (uint.MaxValue - before < value)
                        stat.EssenceEarned = uint.MaxValue;
                    else
                        stat.EssenceEarned = before + value;

                    entry.Property(x => x.EssenceEarned).IsModified = true;
                }

                if (data.EssenceSpent.HasValue)
                {
                    var before = dbStatistics.EssenceSpent;
                    var value = data.EssenceSpent.Value;

                    if (uint.MaxValue - before < value)
                        stat.EssenceSpent = uint.MaxValue;
                    else
                        stat.EssenceSpent = before + value;

                    entry.Property(x => x.EssenceSpent).IsModified = true;
                }

                if (data.ChestsEarned.HasValue)
                {
                    var before = dbStatistics.ChestsEarned;
                    var value = data.ChestsEarned.Value;

                    if (uint.MaxValue - before < value)
                        stat.ChestsEarned = uint.MaxValue;
                    else
                        stat.ChestsEarned = before + value;

                    entry.Property(x => x.ChestsEarned).IsModified = true;
                }

                if (data.ChestsOpened.HasValue)
                {
                    var before = dbStatistics.ChestsOpened;
                    var value = data.ChestsOpened.Value;

                    if (uint.MaxValue - before < value)
                        stat.ChestsOpened = uint.MaxValue;
                    else
                        stat.ChestsOpened = before + value;

                    entry.Property(x => x.ChestsOpened).IsModified = true;
                }

                if (data.SpheresEarned.HasValue)
                {
                    var before = dbStatistics.SpheresEarned;
                    var value = data.SpheresEarned.Value;

                    if (uint.MaxValue - before < value)
                        stat.SpheresEarned = uint.MaxValue;
                    else
                        stat.SpheresEarned = before + value;

                    entry.Property(x => x.SpheresEarned).IsModified = true;
                }

                if (data.SpheresOpened.HasValue)
                {
                    var before = dbStatistics.SpheresOpened;
                    var value = data.SpheresOpened.Value;

                    if (uint.MaxValue - before < value)
                        stat.SpheresOpened = uint.MaxValue;
                    else
                        stat.SpheresOpened = before + value;

                    entry.Property(x => x.SpheresOpened).IsModified = true;
                }

                if (data.CapsulesEarned.HasValue)
                {
                    var before = dbStatistics.CapsulesEarned;
                    var value = data.CapsulesEarned.Value;

                    if (uint.MaxValue - before < value)
                        stat.CapsulesEarned = uint.MaxValue;
                    else
                        stat.CapsulesEarned = before + value;

                    entry.Property(x => x.CapsulesEarned).IsModified = true;
                }

                if (data.CapsulesOpened.HasValue)
                {
                    var before = dbStatistics.CapsulesOpened;
                    var value = data.CapsulesOpened.Value;

                    if (uint.MaxValue - before < value)
                        stat.CapsulesOpened = uint.MaxValue;
                    else
                        stat.CapsulesOpened = before + value;

                    entry.Property(x => x.CapsulesOpened).IsModified = true;
                }

                if (data.TicketsEarned.HasValue)
                {
                    var before = dbStatistics.TicketsEarned;
                    var value = data.TicketsEarned.Value;

                    if (uint.MaxValue - before < value)
                        stat.TicketsEarned = uint.MaxValue;
                    else
                        stat.TicketsEarned = before + value;

                    entry.Property(x => x.TicketsEarned).IsModified = true;
                }

                if (data.TicketsSpent.HasValue)
                {
                    var before = dbStatistics.TicketsSpent;
                    var value = data.TicketsSpent.Value;

                    if (uint.MaxValue - before < value)
                        stat.TicketsSpent = uint.MaxValue;
                    else
                        stat.TicketsSpent = before + value;

                    entry.Property(x => x.TicketsSpent).IsModified = true;
                }

                if (data.DoubleExpsEarned.HasValue)
                {
                    var before = dbStatistics.DoubleExpsEarned;
                    var value = data.DoubleExpsEarned.Value;

                    if (uint.MaxValue - before < value)
                        stat.DoubleExpsEarned = uint.MaxValue;
                    else
                        stat.DoubleExpsEarned = before + value;

                    entry.Property(x => x.DoubleExpsEarned).IsModified = true;
                }

                if (data.DoubleExpsActivated.HasValue)
                {
                    var before = dbStatistics.DoubleExpsActivated;
                    var value = data.DoubleExpsActivated.Value;

                    if (uint.MaxValue - before < value)
                        stat.DoubleExpsActivated = uint.MaxValue;
                    else
                        stat.DoubleExpsActivated = before + value;

                    entry.Property(x => x.DoubleExpsActivated).IsModified = true;
                }

                if (data.BotRespectsEarned.HasValue)
                {
                    var before = dbStatistics.BotRespectsEarned;
                    var value = data.BotRespectsEarned.Value;

                    if (uint.MaxValue - before < value)
                        stat.BotRespectsEarned = uint.MaxValue;
                    else
                        stat.BotRespectsEarned = before + value;

                    entry.Property(x => x.BotRespectsEarned).IsModified = true;
                }

                if (data.BotRespectsActivated.HasValue)
                {
                    var before = dbStatistics.BotRespectsActivated;
                    var value = data.BotRespectsActivated.Value;

                    if (uint.MaxValue - before < value)
                        stat.BotRespectsActivated = uint.MaxValue;
                    else
                        stat.BotRespectsActivated = before + value;

                    entry.Property(x => x.BotRespectsActivated).IsModified = true;
                }

                if (data.RewindsEarned.HasValue)
                {
                    var before = dbStatistics.RewindsEarned;
                    var value = data.RewindsEarned.Value;

                    if (uint.MaxValue - before < value)
                        stat.RewindsEarned = uint.MaxValue;
                    else
                        stat.RewindsEarned = before + value;

                    entry.Property(x => x.RewindsEarned).IsModified = true;
                }

                if (data.RewindsActivated.HasValue)
                {
                    var before = dbStatistics.RewindsActivated;
                    var value = data.RewindsActivated.Value;

                    if (uint.MaxValue - before < value)
                        stat.RewindsActivated = uint.MaxValue;
                    else
                        stat.RewindsActivated = before + value;

                    entry.Property(x => x.RewindsActivated).IsModified = true;
                }

                if (data.GiftsSent.HasValue)
                {
                    var before = dbStatistics.GiftsSent;
                    var value = data.GiftsSent.Value;

                    if (uint.MaxValue - before < value)
                        stat.GiftsSent = uint.MaxValue;
                    else
                        stat.GiftsSent = before + value;

                    entry.Property(x => x.GiftsSent).IsModified = true;
                }

                if (data.GiftsReceived.HasValue)
                {
                    var before = dbStatistics.GiftsReceived;
                    var value = data.GiftsReceived.Value;

                    if (uint.MaxValue - before < value)
                        stat.GiftsReceived = uint.MaxValue;
                    else
                        stat.GiftsReceived = before + value;

                    entry.Property(x => x.GiftsReceived).IsModified = true;
                }

                if (data.MessagesSent.HasValue)
                {
                    var before = dbStatistics.MessagesSent;
                    var value = data.MessagesSent.Value;

                    if (uint.MaxValue - before < value)
                        stat.MessagesSent = uint.MaxValue;
                    else
                        stat.MessagesSent = before + value;

                    OnMessageCreated?.Invoke(null, new MessageCreatedEventArgs(userId, stat.MessagesSent));

                    entry.Property(x => x.MessagesSent).IsModified = true;
                }

                if (data.BragsDone.HasValue)
                {
                    var before = dbStatistics.MessagesSent;
                    var value = data.MessagesSent.Value;

                    if (uint.MaxValue - before < value)
                        stat.MessagesSent = uint.MaxValue;
                    else
                        stat.MessagesSent = before + value;

                    entry.Property(x => x.MessagesSent).IsModified = true;
                }

                if (data.PurchasedItems.HasValue)
                {
                    var before = dbStatistics.PurchasedItems;
                    var value = data.PurchasedItems.Value;

                    if (uint.MaxValue - before < value)
                        stat.PurchasedItems = uint.MaxValue;
                    else
                        stat.PurchasedItems = before + value;

                    entry.Property(x => x.PurchasedItems).IsModified = true;
                }

                if (data.VoiceUptime.HasValue)
                {
                    var before = dbStatistics.VoiceUptime;
                    var value = data.VoiceUptime.Value;

                    VoiceUptimeEarned?.Invoke(null, new VoiceUptimeEarnedEventArgs(userId, value));

                    if (TimeSpan.MaxValue - before < value)
                        stat.VoiceUptime = TimeSpan.MaxValue;
                    else
                        stat.VoiceUptime = before + value;

                    entry.Property(x => x.VoiceUptime).IsModified = true;
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task<RiftStatistics> GetAsync(ulong userId)
        {
            if (!await EnsureExistsAsync(userId))
                throw new DatabaseException(nameof(GetAsync));

            using (var context = new RiftContext())
            {
                return await context.Statistics
                    .FirstAsync(x => x.UserId == userId);
            }
        }
    }

    public class StatisticData
    {
        public uint? CoinsEarned { get; set; }
        public uint? CoinsSpent { get; set; }
        public uint? TokensEarned { get; set; }
        public uint? TokensSpent { get; set; }
        public uint? EssenceEarned { get; set; }
        public uint? EssenceSpent { get; set; }
        public uint? ChestsEarned { get; set; }
        public uint? ChestsOpened { get; set; }
        public uint? SpheresEarned { get; set; }
        public uint? SpheresOpened { get; set; }
        public uint? CapsulesEarned { get; set; }
        public uint? CapsulesOpened { get; set; }
        public uint? TicketsEarned { get; set; }
        public uint? TicketsSpent { get; set; }
        public uint? DoubleExpsEarned { get; set; }
        public uint? DoubleExpsActivated { get; set; }
        public uint? BotRespectsEarned { get; set; }
        public uint? BotRespectsActivated { get; set; }
        public uint? RewindsEarned { get; set; }
        public uint? RewindsActivated { get; set; }
        public uint? GiftsSent { get; set; }
        public uint? GiftsReceived { get; set; }
        public uint? MessagesSent { get; set; }
        public uint? BragsDone { get; set; }
        public uint? PurchasedItems { get; set; }
        public TimeSpan? VoiceUptime { get; set; }
    }
}
