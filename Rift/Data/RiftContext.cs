using Rift.Configuration;
using Rift.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace Rift.Data
{
    public class RiftContext : DbContext
    {
        public DbSet<RiftUser> Users { get; set; }
        public DbSet<RiftInventory> Inventory { get; set; }
        public DbSet<RiftTimestamp> Timestamps { get; set; }
        public DbSet<RiftLolData> LolData { get; set; }
        public DbSet<RiftStatistics> Statistics { get; set; }
        public DbSet<RiftAchievements> Achievements { get; set; }
        public DbSet<RiftPendingUser> PendingUsers { get; set; }
        public DbSet<ScheduledEvent> ScheduledEvents { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder
                .UseMySql($@"Server={Settings.Database.Host};"
                          + $"Uid={Settings.Database.User};"
                          + $"Pwd={Settings.Database.Password};"
                          + $"Database={Settings.Database.Name};");
    }
}
