﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Rift.Data;

namespace Rift.Migrations
{
    [DbContext(typeof(RiftContext))]
    [Migration("20190222215645_RemovedAchievements")]
    partial class RemovedAchievements
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Rift.Data.Models.RiftCooldowns", b =>
                {
                    b.Property<ulong>("UserId");

                    b.Property<DateTime>("BotRespectTime");

                    b.Property<DateTime>("DoubleExpTime");

                    b.Property<DateTime>("LastAttackTime");

                    b.Property<DateTime>("LastBeingAttackedTime");

                    b.Property<DateTime>("LastBragTime");

                    b.Property<DateTime>("LastDailyChestTime");

                    b.Property<DateTime>("LastGiftTime");

                    b.Property<DateTime>("LastLolAccountUpdateTime");

                    b.Property<DateTime>("LastStoreTime");

                    b.HasKey("UserId");

                    b.ToTable("Cooldowns");
                });

            modelBuilder.Entity("Rift.Data.Models.RiftInventory", b =>
                {
                    b.Property<ulong>("UserId");

                    b.Property<uint>("Capsules");

                    b.Property<uint>("Chests");

                    b.Property<uint>("Coins");

                    b.Property<uint>("PowerupsBotRespect");

                    b.Property<uint>("PowerupsDoubleExp");

                    b.Property<uint>("RareTickets");

                    b.Property<uint>("Spheres");

                    b.Property<uint>("Tokens");

                    b.Property<uint>("UsualTickets");

                    b.HasKey("UserId");

                    b.ToTable("Inventory");
                });

            modelBuilder.Entity("Rift.Data.Models.RiftLolData", b =>
                {
                    b.Property<ulong>("UserId");

                    b.Property<string>("AccountId");

                    b.Property<string>("PlayerUUID");

                    b.Property<string>("SummonerId");

                    b.Property<string>("SummonerName");

                    b.Property<string>("SummonerRegion");

                    b.HasKey("UserId");

                    b.ToTable("LolData");
                });

            modelBuilder.Entity("Rift.Data.Models.RiftPendingUser", b =>
                {
                    b.Property<ulong>("UserId");

                    b.Property<string>("AccountId");

                    b.Property<string>("ConfirmationCode");

                    b.Property<DateTime>("ExpirationTime");

                    b.Property<string>("PlayerUUID");

                    b.Property<string>("Region");

                    b.Property<string>("SummonedId");

                    b.HasKey("UserId");

                    b.ToTable("PendingUsers");
                });

            modelBuilder.Entity("Rift.Data.Models.RiftStatistics", b =>
                {
                    b.Property<ulong>("UserId");

                    b.Property<ulong>("AttacksDone");

                    b.Property<ulong>("AttacksReceived");

                    b.Property<ulong>("BragTotal");

                    b.Property<ulong>("CapsuleEarnedTotal");

                    b.Property<ulong>("CapsuleOpenedTotal");

                    b.Property<ulong>("ChestsEarnedTotal");

                    b.Property<ulong>("ChestsOpenedTotal");

                    b.Property<ulong>("CoinsEarnedTotal");

                    b.Property<ulong>("CoinsSpentTotal");

                    b.Property<ulong>("GiftsReceived");

                    b.Property<ulong>("GiftsSent");

                    b.Property<ulong>("MessagesSentTotal");

                    b.Property<ulong>("PurchasedItemsTotal");

                    b.Property<ulong>("SphereEarnedTotal");

                    b.Property<ulong>("SphereOpenedTotal");

                    b.Property<ulong>("TokensEarnedTotal");

                    b.Property<ulong>("TokensSpentTotal");

                    b.HasKey("UserId");

                    b.ToTable("Statistics");
                });

            modelBuilder.Entity("Rift.Data.Models.RiftTempRole", b =>
                {
                    b.Property<ulong>("UserId");

                    b.Property<ulong>("RoleId");

                    b.Property<DateTime>("ExpirationTime");

                    b.Property<string>("ObtainedFrom");

                    b.Property<DateTime>("ObtainedTime");

                    b.HasKey("UserId", "RoleId");

                    b.ToTable("TempRoles");
                });

            modelBuilder.Entity("Rift.Data.Models.RiftUser", b =>
                {
                    b.Property<ulong>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Donate");

                    b.Property<uint>("Experience");

                    b.Property<uint>("Level");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Rift.Data.Models.ScheduledEvent", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("DayId");

                    b.Property<int>("EventId");

                    b.Property<int>("Hour");

                    b.Property<int>("Minute");

                    b.HasKey("Id");

                    b.ToTable("ScheduledEvents");
                });

            modelBuilder.Entity("Rift.Data.Models.RiftCooldowns", b =>
                {
                    b.HasOne("Rift.Data.Models.RiftUser", "User")
                        .WithOne("Cooldowns")
                        .HasForeignKey("Rift.Data.Models.RiftCooldowns", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Rift.Data.Models.RiftInventory", b =>
                {
                    b.HasOne("Rift.Data.Models.RiftUser", "User")
                        .WithOne("Inventory")
                        .HasForeignKey("Rift.Data.Models.RiftInventory", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Rift.Data.Models.RiftLolData", b =>
                {
                    b.HasOne("Rift.Data.Models.RiftUser", "User")
                        .WithOne("LolData")
                        .HasForeignKey("Rift.Data.Models.RiftLolData", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Rift.Data.Models.RiftPendingUser", b =>
                {
                    b.HasOne("Rift.Data.Models.RiftUser", "User")
                        .WithOne("PendingUser")
                        .HasForeignKey("Rift.Data.Models.RiftPendingUser", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Rift.Data.Models.RiftStatistics", b =>
                {
                    b.HasOne("Rift.Data.Models.RiftUser", "User")
                        .WithOne("Statistics")
                        .HasForeignKey("Rift.Data.Models.RiftStatistics", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Rift.Data.Models.RiftTempRole", b =>
                {
                    b.HasOne("Rift.Data.Models.RiftUser", "User")
                        .WithMany("TempRoles")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_RiftTempRoles_Users_UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
