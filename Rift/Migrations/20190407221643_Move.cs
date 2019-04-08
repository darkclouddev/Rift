using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class Move : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScheduledEvents",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
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
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<decimal>(nullable: false),
                    Experience = table.Column<long>(nullable: false),
                    Level = table.Column<long>(nullable: false),
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
                    UserId = table.Column<decimal>(nullable: false),
                    LastStoreTime = table.Column<DateTime>(nullable: false),
                    LastAttackTime = table.Column<DateTime>(nullable: false),
                    LastBeingAttackedTime = table.Column<DateTime>(nullable: false),
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
                    UserId = table.Column<decimal>(nullable: false),
                    Coins = table.Column<long>(nullable: false),
                    Tokens = table.Column<long>(nullable: false),
                    Chests = table.Column<long>(nullable: false),
                    Capsules = table.Column<long>(nullable: false),
                    Spheres = table.Column<long>(nullable: false),
                    PowerupsDoubleExp = table.Column<long>(nullable: false),
                    PowerupsBotRespect = table.Column<long>(nullable: false),
                    UsualTickets = table.Column<long>(nullable: false),
                    RareTickets = table.Column<long>(nullable: false)
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
                    UserId = table.Column<decimal>(nullable: false),
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
                    UserId = table.Column<decimal>(nullable: false),
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
                    UserId = table.Column<decimal>(nullable: false),
                    CoinsEarnedTotal = table.Column<long>(nullable: false),
                    TokensEarnedTotal = table.Column<long>(nullable: false),
                    ChestsEarnedTotal = table.Column<long>(nullable: false),
                    SphereEarnedTotal = table.Column<long>(nullable: false),
                    CapsuleEarnedTotal = table.Column<long>(nullable: false),
                    ChestsOpenedTotal = table.Column<long>(nullable: false),
                    SphereOpenedTotal = table.Column<long>(nullable: false),
                    CapsuleOpenedTotal = table.Column<long>(nullable: false),
                    AttacksDone = table.Column<long>(nullable: false),
                    AttacksReceived = table.Column<long>(nullable: false),
                    CoinsSpentTotal = table.Column<long>(nullable: false),
                    TokensSpentTotal = table.Column<long>(nullable: false),
                    GiftsSent = table.Column<long>(nullable: false),
                    GiftsReceived = table.Column<long>(nullable: false),
                    MessagesSentTotal = table.Column<long>(nullable: false),
                    BragTotal = table.Column<long>(nullable: false),
                    PurchasedItemsTotal = table.Column<long>(nullable: false)
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
                    UserId = table.Column<decimal>(nullable: false),
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
                    UserId = table.Column<decimal>(nullable: false),
                    RoleId = table.Column<decimal>(nullable: false),
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
                name: "PendingUsers");

            migrationBuilder.DropTable(
                name: "ScheduledEvents");

            migrationBuilder.DropTable(
                name: "Statistics");

            migrationBuilder.DropTable(
                name: "Streamers");

            migrationBuilder.DropTable(
                name: "TempRoles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }

}
