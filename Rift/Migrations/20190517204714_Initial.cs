using System;

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "MessageMappings",
                table => new
                {
                    Identifier = table.Column<string>(nullable: false, maxLength: 64),
                    MessageId = table.Column<int>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_MessageMappings", x => x.Identifier); });

            migrationBuilder.CreateTable(
                "ScheduledEvents",
                table => new
                {
                    Id = table.Column<uint>(nullable: false)
                              .Annotation("MySql:ValueGenerationStrategy",
                                          MySqlValueGenerationStrategy.IdentityColumn),
                    DayId = table.Column<int>(nullable: false),
                    EventId = table.Column<int>(nullable: false),
                    Hour = table.Column<int>(nullable: false),
                    Minute = table.Column<int>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_ScheduledEvents", x => x.Id); });

            migrationBuilder.CreateTable(
                "StoredMessages",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                              .Annotation("MySql:ValueGenerationStrategy",
                                          MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    Embed = table.Column<string>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true),
                    ApplyFormat = table.Column<bool>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_StoredMessages", x => x.Id); });

            migrationBuilder.CreateTable(
                "Users",
                table => new
                {
                    UserId = table.Column<ulong>(nullable: false)
                                  .Annotation("MySql:ValueGenerationStrategy",
                                              MySqlValueGenerationStrategy.IdentityColumn),
                    Experience = table.Column<uint>(nullable: false),
                    Level = table.Column<uint>(nullable: false),
                    Donate = table.Column<decimal>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Users", x => x.UserId); });

            migrationBuilder.CreateTable(
                "Cooldowns",
                table => new
                {
                    UserId = table.Column<ulong>(nullable: false),
                    LastStoreTime = table.Column<DateTime>(nullable: false),
                    LastDailyChestTime = table.Column<DateTime>(nullable: false),
                    LastBragTime = table.Column<DateTime>(nullable: false),
                    LastGiftTime = table.Column<DateTime>(nullable: false),
                    DoubleExpTime = table.Column<DateTime>(nullable: false),
                    BotRespectTime = table.Column<DateTime>(nullable: false),
                    LastLolAccountUpdateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cooldowns", x => x.UserId);
                    table.ForeignKey(
                        "FK_Cooldowns_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Inventory",
                table => new
                {
                    UserId = table.Column<ulong>(nullable: false),
                    Coins = table.Column<uint>(nullable: false),
                    Tokens = table.Column<uint>(nullable: false),
                    Chests = table.Column<uint>(nullable: false),
                    Capsules = table.Column<uint>(nullable: false),
                    Spheres = table.Column<uint>(nullable: false),
                    PowerupsDoubleExp = table.Column<uint>(nullable: false),
                    PowerupsBotRespect = table.Column<uint>(nullable: false),
                    UsualTickets = table.Column<uint>(nullable: false),
                    RareTickets = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.UserId);
                    table.ForeignKey(
                        "FK_Inventory_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "LolData",
                table => new
                {
                    UserId = table.Column<ulong>(nullable: false),
                    SummonerRegion = table.Column<string>(nullable: true),
                    PlayerUUID = table.Column<string>(nullable: true),
                    AccountId = table.Column<string>(nullable: true),
                    SummonerId = table.Column<string>(nullable: true),
                    SummonerName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LolData", x => x.UserId);
                    table.ForeignKey(
                        "FK_LolData_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "PendingUsers",
                table => new
                {
                    UserId = table.Column<ulong>(nullable: false),
                    Region = table.Column<string>(nullable: true),
                    PlayerUUID = table.Column<string>(nullable: true),
                    AccountId = table.Column<string>(nullable: true),
                    SummonedId = table.Column<string>(nullable: true),
                    ConfirmationCode = table.Column<string>(nullable: true),
                    ExpirationTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingUsers", x => x.UserId);
                    table.ForeignKey(
                        "FK_PendingUsers_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Statistics",
                table => new
                {
                    UserId = table.Column<ulong>(nullable: false),
                    CoinsEarnedTotal = table.Column<uint>(nullable: false),
                    TokensEarnedTotal = table.Column<uint>(nullable: false),
                    ChestsEarnedTotal = table.Column<uint>(nullable: false),
                    SphereEarnedTotal = table.Column<uint>(nullable: false),
                    CapsuleEarnedTotal = table.Column<uint>(nullable: false),
                    ChestsOpenedTotal = table.Column<uint>(nullable: false),
                    SphereOpenedTotal = table.Column<uint>(nullable: false),
                    CapsuleOpenedTotal = table.Column<uint>(nullable: false),
                    AttacksDone = table.Column<uint>(nullable: false),
                    AttacksReceived = table.Column<uint>(nullable: false),
                    CoinsSpentTotal = table.Column<uint>(nullable: false),
                    TokensSpentTotal = table.Column<uint>(nullable: false),
                    GiftsSent = table.Column<uint>(nullable: false),
                    GiftsReceived = table.Column<uint>(nullable: false),
                    MessagesSentTotal = table.Column<uint>(nullable: false),
                    BragTotal = table.Column<uint>(nullable: false),
                    PurchasedItemsTotal = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistics", x => x.UserId);
                    table.ForeignKey(
                        "FK_Statistics_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Streamers",
                table => new
                {
                    UserId = table.Column<ulong>(nullable: false),
                    PictureUrl = table.Column<string>(nullable: true),
                    StreamUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Streamers", x => x.UserId);
                    table.ForeignKey(
                        "FK_Streamers_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "TempRoles",
                table => new
                {
                    UserId = table.Column<ulong>(nullable: false),
                    RoleId = table.Column<ulong>(nullable: false),
                    ObtainedTime = table.Column<DateTime>(nullable: false),
                    ObtainedFrom = table.Column<string>(nullable: true),
                    ExpirationTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempRoles", x => new {x.UserId, x.RoleId});
                    table.ForeignKey(
                        "FK_RiftTempRoles_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "UserId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Cooldowns");

            migrationBuilder.DropTable(
                "Inventory");

            migrationBuilder.DropTable(
                "LolData");

            migrationBuilder.DropTable(
                "MessageMappings");

            migrationBuilder.DropTable(
                "PendingUsers");

            migrationBuilder.DropTable(
                "ScheduledEvents");

            migrationBuilder.DropTable(
                "Statistics");

            migrationBuilder.DropTable(
                "StoredMessages");

            migrationBuilder.DropTable(
                "Streamers");

            migrationBuilder.DropTable(
                "TempRoles");

            migrationBuilder.DropTable(
                "Users");
        }
    }
}
