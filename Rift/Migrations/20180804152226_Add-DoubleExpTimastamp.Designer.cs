﻿// <auto-generated />
using Rift.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    [DbContext(typeof(RiftContext))]
    [Migration("20180804152226_Add-DoubleExpTimastamp")]
    partial class AddDoubleExpTimastamp
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .HasAnnotation("ProductVersion", "2.0.3-rtm-10026");

            modelBuilder.Entity("IonicLib.Database.DbUser", b =>
                {
                    b.Property<ulong>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<uint>("AttacksDone");

                    b.Property<uint>("AttacksReceived");

                    b.Property<uint>("Capsules");

                    b.Property<uint>("Chests");

                    b.Property<uint>("Coins");

                    b.Property<uint>("CoinsEarnedTotal");

                    b.Property<uint>("CoinsSpentTotal");

                    b.Property<ulong>("CreatedAtTimestamp");

                    b.Property<uint>("CustomTickets");

                    b.Property<decimal>("Donate");

                    b.Property<ulong>("DoubleExpTimestamp");

                    b.Property<uint>("Experience");

                    b.Property<uint>("GiftsReceived");

                    b.Property<uint>("GiftsSent");

                    b.Property<uint>("GiveawayTickets");

                    b.Property<ulong>("LastAttackTimestamp");

                    b.Property<ulong>("LastBragTimestamp");

                    b.Property<ulong>("LastDailyChestTimestamp");

                    b.Property<ulong>("LastGiftTimestamp");

                    b.Property<ulong>("LastStoreTimestamp");

                    b.Property<uint>("Level");

                    b.Property<long>("LolAccountId");

                    b.Property<long>("LolSummonerId");

                    b.Property<string>("LolSummonerName");

                    b.Property<string>("LolSummonerRegion");

                    b.Property<uint>("PowerupsAttack");

                    b.Property<uint>("PowerupsBotRespect");

                    b.Property<uint>("PowerupsDoubleExp");

                    b.Property<uint>("PowerupsProtection");

                    b.Property<uint>("Spheres");

                    b.Property<uint>("Tokens");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("IonicLib.Database.ScheduledEvent", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DayId");

                    b.Property<int>("EventId");

                    b.Property<int>("Hour");

                    b.Property<int>("Minute");

                    b.HasKey("Id");

                    b.ToTable("Events");
                });
#pragma warning restore 612, 618
        }
    }
}
