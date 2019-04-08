﻿using Rift.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace Rift.Data
{
    public class RiftContext : DbContext
    {
        public DbSet<RiftUser> Users { get; set; }
        public DbSet<RiftInventory> Inventory { get; set; }
        public DbSet<RiftCooldowns> Cooldowns { get; set; }
        public DbSet<RiftLolData> LolData { get; set; }
        public DbSet<RiftStatistics> Statistics { get; set; }
        public DbSet<RiftPendingUser> PendingUsers { get; set; }
        public DbSet<ScheduledEvent> ScheduledEvents { get; set; }
        public DbSet<RiftTempRole> TempRoles { get; set; }
        public DbSet<RiftStreamer> Streamers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlServer(@"Data Source=127.0.0.1,1434;Initial Catalog=Rift;persist security info=True;user id=Rift;password=727");

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<RiftUser>().HasKey(key => key.UserId);

            builder.Entity<RiftCooldowns>().HasKey(key => key.UserId);
            builder.Entity<RiftUser>()
                   .HasOne(user => user.Cooldowns)
                   .WithOne(cd => cd.User)
                   .HasForeignKey<RiftCooldowns>(user => user.UserId);

            builder.Entity<RiftInventory>().HasKey(key => key.UserId);
            builder.Entity<RiftUser>()
                   .HasOne(user => user.Inventory)
                   .WithOne(inv => inv.User)
                   .HasForeignKey<RiftInventory>(user => user.UserId);

            builder.Entity<RiftLolData>().HasKey(key => key.UserId);
            builder.Entity<RiftUser>()
                   .HasOne(user => user.LolData)
                   .WithOne(lol => lol.User)
                   .HasForeignKey<RiftLolData>(user => user.UserId);

            builder.Entity<RiftPendingUser>().HasKey(key => key.UserId);
            builder.Entity<RiftUser>()
                   .HasOne(user => user.PendingUser)
                   .WithOne(pending => pending.User)
                   .HasForeignKey<RiftPendingUser>(user => user.UserId);

            builder.Entity<RiftTempRole>().ToTable("TempRoles");
            builder.Entity<RiftTempRole>()
                   .HasKey(x => new
                   {
                       x.UserId,
                       x.RoleId
                   });
            
            builder.Entity<RiftUser>()
                   .HasMany(users => users.TempRoles)
                   .WithOne(role => role.User)
                   .HasForeignKey(role => role.UserId)
                   .HasConstraintName($"FK_RiftTempRoles_Users_UserId");

            builder.Entity<RiftStatistics>().HasKey(key => key.UserId);
            builder.Entity<RiftUser>()
                   .HasOne(user => user.Statistics)
                   .WithOne(stat => stat.User)
                   .HasForeignKey<RiftStatistics>(user => user.UserId);

            builder.Entity<ScheduledEvent>().HasKey(key => key.Id);
            builder.Entity<ScheduledEvent>()
                   .Property(prop => prop.Id)
                   .ValueGeneratedOnAdd();

            builder.Entity<RiftStreamer>().ToTable("Streamers");
            builder.Entity<RiftStreamer>().HasKey(x => x.UserId);
            builder.Entity<RiftUser>()
                   .HasOne(user => user.Streamers)
                   .WithOne(streamer => streamer.User)
                   .HasForeignKey<RiftStreamer>(user => user.UserId);
        }
    }
}
