using Rift.Data.Models;

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
        public DbSet<RiftScheduledEvent> ScheduledEvents { get; set; }
        public DbSet<RiftTempRole> TempRoles { get; set; }
        public DbSet<RiftStreamer> Streamers { get; set; }
        public DbSet<RiftMessage> StoredMessages { get; set; }
        public DbSet<RiftMapping> MessageMappings { get; set; }
        public DbSet<RiftToxicity> Toxicity { get; set; }
        public DbSet<RiftModerationLog> ModerationLog { get; set; }
        public DbSet<RiftSettings> Settings { get; set; }
        public DbSet<RiftRoleInventory> RoleInventories { get; set; }
        public DbSet<RiftSystemTimer> SystemCooldowns { get; set; }
        public DbSet<RiftReward> Rewards { get; set; }
        public DbSet<RiftGiveaway> Giveaways { get; set; }
        public DbSet<RiftGiveawayLog> GiveawayLogs { get; set; }
        public DbSet<RiftGiveawayActive> ActiveGiveaways { get; set; }
        public DbSet<RiftStage> QuestStages { get; set; }
        public DbSet<RiftQuest> Quests { get; set; }
        public DbSet<RiftQuestProgress> QuestProgress { get; set; }
        public DbSet<RiftProfileBackground> ProfileBackgrounds { get; set; }
        public DbSet<RiftBackgroundInventory> BackgroundInventories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseMySql(@"Server=localhost;Database=rift;Uid=rift;Pwd=;");

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
                .HasConstraintName("FK_RiftTempRoles_Users_UserId");

            builder.Entity<RiftStatistics>().HasKey(key => key.UserId);
            builder.Entity<RiftUser>()
                .HasOne(user => user.Statistics)
                .WithOne(stat => stat.User)
                .HasForeignKey<RiftStatistics>(user => user.UserId);

            builder.Entity<RiftScheduledEvent>().HasKey(key => key.Id);
            builder.Entity<RiftScheduledEvent>().Property(prop => prop.Id).ValueGeneratedOnAdd();

            builder.Entity<RiftStreamer>().ToTable("Streamers");
            builder.Entity<RiftStreamer>().HasKey(x => x.UserId);
            builder.Entity<RiftUser>()
                .HasOne(user => user.Streamers)
                .WithOne(streamer => streamer.User)
                .HasForeignKey<RiftStreamer>(user => user.UserId);

            builder.Entity<RiftMapping>().HasKey(key => key.Identifier);
            
            builder.Entity<RiftMessage>().HasKey(key => key.Id);
            builder.Entity<RiftMessage>().Property(key => key.Id).ValueGeneratedOnAdd();

            builder.Entity<RiftToxicity>().HasKey(key => key.UserId);
            builder.Entity<RiftToxicity>().Ignore(key => key.Level);
            builder.Entity<RiftUser>().HasOne(user => user.Toxicity)
                .WithOne(toxicity => toxicity.User)
                .HasForeignKey<RiftToxicity>(user => user.UserId);

            builder.Entity<RiftModerationLog>().HasKey(prop => prop.Id);
            builder.Entity<RiftModerationLog>().Property(prop => prop.Id).ValueGeneratedOnAdd();

            builder.Entity<RiftSettings>().HasKey(prop => prop.Id);
            builder.Entity<RiftSettings>().Property(prop => prop.Id).ValueGeneratedNever();

            builder.Entity<RiftRoleInventory>()
                .HasKey(x => new
                {
                    x.UserId,
                    x.RoleId
                });

            builder.Entity<RiftSystemTimer>().HasKey(prop => prop.Id);
            builder.Entity<RiftSystemTimer>().Property(prop => prop.Id).ValueGeneratedOnAdd();

            builder.Entity<RiftReward>().HasKey(prop => prop.Id);
            builder.Entity<RiftReward>().Property(prop => prop.Id).ValueGeneratedOnAdd();
            builder.Entity<RiftReward>().Ignore(prop => prop.ItemReward);
            builder.Entity<RiftReward>().Ignore(prop => prop.RoleReward);

            builder.Entity<RiftGiveaway>().HasKey(prop => prop.Name);

            builder.Entity<RiftGiveawayLog>().HasKey(prop => prop.Id);
            builder.Entity<RiftGiveawayLog>().Property(prop => prop.Id).ValueGeneratedOnAdd();
            builder.Entity<RiftGiveawayLog>().Ignore(prop => prop.Winners);
            builder.Entity<RiftGiveawayLog>().Ignore(prop => prop.Participants);

            builder.Entity<RiftGiveawayActive>().HasKey(prop => prop.Id);
            builder.Entity<RiftGiveawayActive>().Property(prop => prop.Id).ValueGeneratedOnAdd();

            builder.Entity<RiftStage>().HasKey(prop => prop.Id);

            builder.Entity<RiftQuest>().HasKey(prop => prop.Id);

            builder.Entity<RiftQuestProgress>()
                .HasKey(prop => new
                {
                    prop.UserId,
                    prop.QuestId
                });
            builder.Entity<RiftQuestProgress>().Property(prop => prop.UserId).ValueGeneratedNever();
            builder.Entity<RiftQuestProgress>().Property(prop => prop.QuestId).ValueGeneratedNever();
            builder.Entity<RiftQuestProgress>().Ignore(x => x.TotalMonstersKilled);

            builder.Entity<RiftProfileBackground>().HasKey(x => x.Id);
            builder.Entity<RiftProfileBackground>().Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Entity<RiftBackgroundInventory>()
                .HasKey(prop => new
                {
                    prop.UserId,
                    prop.BackgroundId
                });
            builder.Entity<RiftUser>()
                .HasOne(user => user.BackgroundInventory)
                .WithOne(inv => inv.User)
                .HasForeignKey<RiftBackgroundInventory>(inv => inv.UserId);
        }
    }
}
