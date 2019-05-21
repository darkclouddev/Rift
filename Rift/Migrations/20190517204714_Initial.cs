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
                name: "MessageMappings",
                columns: table => new
                {
                    Identifier = table.Column<string>(nullable: false, maxLength: 64),
                    MessageId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageMappings", x => x.Identifier);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledEvents",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DayId = table.Column<int>(nullable: false),
                    EventId = table.Column<int>(nullable: false),
                    Hour = table.Column<int>(nullable: false),
                    Minute = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoredMessages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    Embed = table.Column<string>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true),
                    ApplyFormat = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<ulong>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Experience = table.Column<uint>(nullable: false),
                    Level = table.Column<uint>(nullable: false),
                    Donate = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Cooldowns",
                columns: table => new
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
                        name: "FK_Cooldowns_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
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
                        name: "FK_Inventory_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LolData",
                columns: table => new
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
                        name: "FK_LolData_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PendingUsers",
                columns: table => new
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
                        name: "FK_PendingUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Statistics",
                columns: table => new
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
                        name: "FK_Statistics_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Streamers",
                columns: table => new
                {
                    UserId = table.Column<ulong>(nullable: false),
                    PictureUrl = table.Column<string>(nullable: true),
                    StreamUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Streamers", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Streamers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TempRoles",
                columns: table => new
                {
                    UserId = table.Column<ulong>(nullable: false),
                    RoleId = table.Column<ulong>(nullable: false),
                    ObtainedTime = table.Column<DateTime>(nullable: false),
                    ObtainedFrom = table.Column<string>(nullable: true),
                    ExpirationTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_RiftTempRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cooldowns");

            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropTable(
                name: "LolData");

            migrationBuilder.DropTable(
                name: "MessageMappings");

            migrationBuilder.DropTable(
                name: "PendingUsers");

            migrationBuilder.DropTable(
                name: "ScheduledEvents");

            migrationBuilder.DropTable(
                name: "Statistics");

            migrationBuilder.DropTable(
                name: "StoredMessages");

            migrationBuilder.DropTable(
                name: "Streamers");

            migrationBuilder.DropTable(
                name: "TempRoles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
