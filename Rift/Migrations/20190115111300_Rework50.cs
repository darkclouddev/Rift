using System;

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class Rework50 : Migration
    {
        protected override void Up(MigrationBuilder builder)
        {
            // deprecated fields removal
            builder.DropColumn(name: "GiftSphere", table: "Users");
            builder.DropColumn(name: "CreatedAtTimestamp", table: "Users");
            builder.DropColumn(name: "PowerupsAttack", table: "Users");

            builder.CreateTable(name: "Achievements",
                                columns: table => new
                                {
                                    UserId = table.Column<ulong>(nullable: false),
                                    Write100Messages = table.Column<bool>(nullable: false),
                                    Write1000Messages = table.Column<bool>(nullable: false),
                                    Reach10Level = table.Column<bool>(nullable: false),
                                    Reach30Level = table.Column<bool>(nullable: false),
                                    Brag100Times = table.Column<bool>(nullable: false),
                                    Attack200Times = table.Column<bool>(nullable: false),
                                    OpenSphere = table.Column<bool>(nullable: false),
                                    Purchase200Items = table.Column<bool>(nullable: false),
                                    Open100Chests = table.Column<bool>(nullable: false),
                                    Send100Gifts = table.Column<bool>(nullable: false),
                                    IsDonator = table.Column<bool>(nullable: false),
                                    HasDonatedRole = table.Column<bool>(nullable: false),
                                    GiftToBotKeeper = table.Column<bool>(nullable: false),
                                    GiftToModerator = table.Column<bool>(nullable: false),
                                    AttackWise = table.Column<bool>(nullable: false)
                                },
                                constraints: table =>
                                {
                                    table.PrimaryKey("PK_Achievements", x => x.UserId);
                                    table.ForeignKey(name: "FK_Achievements_Users_UserId",
                                                     column: x => x.UserId,
                                                     principalTable: "Users",
                                                     principalColumn: "UserId",
                                                     onDelete: ReferentialAction.Cascade);
                                });

            builder.Sql(@"INSERT INTO `Achievements`(UserId,
                                                     Write100Messages,
                                                     Write1000Messages,
                                                     Reach10Level,
                                                     Reach30Level,
                                                     Brag100Times,
                                                     Attack200Times,
                                                     OpenSphere,
                                                     Purchase200Items,
                                                     Open100Chests,
                                                     Send100Gifts,
                                                     IsDonator,
                                                     HasDonatedRole,
                                                     GiftToBotKeeper,
                                                     GiftToModerator,
                                                     AttackWise)
                        SELECT UserId,
                               Write100Mesages,
                               Write1000Mesages,
                               Reach10Level,
                               Reach30Level,
                               Brag100times,
                               Attack200times,
                               OpenSphere,
                               Purchaise200Items,
                               Open100Chests,
                               Send100Gifts,
                               IsDonater,
                               HasDonatedRole,
                               GiftToBotKeeper,
                               GiftToModerator,
                               AttackWise
                        FROM `Users`");

            builder.DropColumn(name: "Write1000Mesages", table: "Users");
            builder.DropColumn(name: "Write100Mesages", table: "Users");
            builder.DropColumn(name: "Reach10Level", table: "Users");
            builder.DropColumn(name: "Reach30Level", table: "Users");
            builder.DropColumn(name: "Brag100times", table: "Users");
            builder.DropColumn(name: "Attack200times", table: "Users");
            builder.DropColumn(name: "OpenSphere", table: "Users");
            builder.DropColumn(name: "Purchaise200Items", table: "Users");
            builder.DropColumn(name: "Open100Chests", table: "Users");
            builder.DropColumn(name: "Send100Gifts", table: "Users");
            builder.DropColumn(name: "IsDonater", table: "Users");
            builder.DropColumn(name: "HasDonatedRole", table: "Users");
            builder.DropColumn(name: "GiftToBotKeeper", table: "Users");
            builder.DropColumn(name: "GiftToModerator", table: "Users");
            builder.DropColumn(name: "AttackWise", table: "Users");

            builder.CreateTable(name: "Cooldowns",
                                columns: table => new
                                {
                                    UserId = table.Column<ulong>(nullable: false),
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
                                    table.ForeignKey(name: "FK_Cooldowns_Users_UserId",
                                                     column: x => x.UserId,
                                                     principalTable: "Users",
                                                     principalColumn: "UserId",
                                                     onDelete: ReferentialAction.Cascade);
                                });

            builder.Sql(@"INSERT INTO `Cooldowns` (UserId,
                                                   LastStoreTime,
                                                   LastAttackTime,
                                                   LastBeingAttackedTime,
                                                   LastDailyChestTime,
                                                   LastBragTime,
                                                   LastGiftTime,
                                                   DoubleExpTime,
                                                   BotRespectTime,
                                                   LastLolAccountUpdateTime)
                         SELECT UserId,
                                FROM_UNIXTIME(LastStoreTimestamp),
                                FROM_UNIXTIME(LastAttackTimestamp),
                                FROM_UNIXTIME(0),
                                FROM_UNIXTIME(LastDailyChestTimestamp),
                                FROM_UNIXTIME(LastBragTimestamp),
                                FROM_UNIXTIME(LastGiftTimestamp),
                                FROM_UNIXTIME(DoubleExpTimestamp),
                                FROM_UNIXTIME(BotRespectTimestamp),
                                FROM_UNIXTIME(LastLolAccountUpdateTimestamp)
                         FROM `Users`;");

            builder.DropColumn(name: "LastStoreTimestamp", table: "Users");
            builder.DropColumn(name: "LastAttackTimestamp", table: "Users");
            builder.DropColumn(name: "LastDailyChestTimestamp", table: "Users");
            builder.DropColumn(name: "LastBragTimestamp", table: "Users");
            builder.DropColumn(name: "LastGiftTimestamp", table: "Users");
            builder.DropColumn(name: "DoubleExpTimestamp", table: "Users");
            builder.DropColumn(name: "BotRespectTimestamp", table: "Users");
            builder.DropColumn(name: "LastLolAccountUpdateTimestamp", table: "Users");

            builder.CreateTable(name: "Inventory",
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
                                    table.ForeignKey(name: "FK_Inventory_Users_UserId",
                                                     column: x => x.UserId,
                                                     principalTable: "Users",
                                                     principalColumn: "UserId",
                                                     onDelete: ReferentialAction.Cascade);
                                });

            builder.Sql(@"INSERT INTO `Inventory` (UserId,
                                                    Coins,
                                                    Tokens,
                                                    Chests,
                                                    Capsules,
                                                    Spheres,
                                                    PowerupsDoubleExp,
                                                    PowerupsBotRespect,
                                                    UsualTickets,
                                                    RareTickets)
                          SELECT UserId,
                                 Coins,
                                 Tokens,
                                 Chests,
                                 Capsules,
                                 Spheres,
                                 PowerupsDoubleExp,
                                 PowerupsBotRespect,
                                 CustomTickets,
                                 GiveawayTickets
                          FROM `Users`;");

            builder.DropColumn(name: "Coins", table: "Users");
            builder.DropColumn(name: "Tokens", table: "Users");
            builder.DropColumn(name: "Chests", table: "Users");
            builder.DropColumn(name: "Capsules", table: "Users");
            builder.DropColumn(name: "Spheres", table: "Users");
            builder.DropColumn(name: "PowerupsDoubleExp", table: "Users");
            builder.DropColumn(name: "PowerupsBotRespect", table: "Users");
            builder.DropColumn(name: "CustomTickets", table: "Users");
            builder.DropColumn(name: "GiveawayTickets", table: "Users");

            builder.CreateTable(name: "LolData",
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
                                    table.ForeignKey(name: "FK_LolData_Users_UserId",
                                                     column: x => x.UserId,
                                                     principalTable: "Users",
                                                     principalColumn: "UserId",
                                                     onDelete: ReferentialAction.Cascade);
                                });

            builder.DropColumn(name: "LolAccountId", table: "Users");
            builder.DropColumn(name: "LolSummonerId", table: "Users");
            builder.DropColumn(name: "LolSummonerName", table: "Users");
            builder.DropColumn(name: "LolSummonerRegion", table: "Users");

            builder.CreateTable(name: "PendingUsers",
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
                                    table.ForeignKey(name: "FK_PendingUsers_Users_UserId",
                                                     column: x => x.UserId,
                                                     principalTable: "Users",
                                                     principalColumn: "UserId",
                                                     onDelete: ReferentialAction.Cascade);
                                });

            builder.CreateTable(name: "ScheduledEvents",
                                columns: table => new
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

            builder.Sql(@"INSERT INTO `ScheduledEvents`(Id, DayId, EventId, `Hour`, `Minute`)
                        SELECT Id, DayId, EventId, `Hour`, `Minute`
                        FROM `Events`");

            builder.DropTable(name: "Events");

            builder.CreateTable(name: "Statistics",
                                columns: table => new
                                {
                                    UserId = table.Column<ulong>(nullable: false),
                                    CoinsEarnedTotal = table.Column<ulong>(nullable: false),
                                    TokensEarnedTotal = table.Column<ulong>(nullable: false),
                                    ChestsEarnedTotal = table.Column<ulong>(nullable: false),
                                    SphereEarnedTotal = table.Column<ulong>(nullable: false),
                                    CapsuleEarnedTotal = table.Column<ulong>(nullable: false),
                                    ChestsOpenedTotal = table.Column<ulong>(nullable: false),
                                    SphereOpenedTotal = table.Column<ulong>(nullable: false),
                                    CapsuleOpenedTotal = table.Column<ulong>(nullable: false),
                                    AttacksDone = table.Column<ulong>(nullable: false),
                                    AttacksReceived = table.Column<ulong>(nullable: false),
                                    CoinsSpentTotal = table.Column<ulong>(nullable: false),
                                    TokensSpentTotal = table.Column<ulong>(nullable: false),
                                    GiftsSent = table.Column<ulong>(nullable: false),
                                    GiftsReceived = table.Column<ulong>(nullable: false),
                                    MessagesSentTotal = table.Column<ulong>(nullable: false),
                                    BragTotal = table.Column<ulong>(nullable: false),
                                    PurchasedItemsTotal = table.Column<ulong>(nullable: false)
                                },
                                constraints: table =>
                                {
                                    table.PrimaryKey("PK_Statistics", x => x.UserId);
                                    table.ForeignKey(name: "FK_Statistics_Users_UserId",
                                                     column: x => x.UserId,
                                                     principalTable: "Users",
                                                     principalColumn: "UserId",
                                                     onDelete: ReferentialAction.Cascade);
                                });

            builder.Sql(@"INSERT INTO `Statistics` (UserId,
                                                    CoinsEarnedTotal,
                                                    TokensEarnedTotal,
                                                    ChestsEarnedTotal,
                                                    SphereEarnedTotal,
                                                    CapsuleEarnedTotal,
                                                    ChestsOpenedTotal,
                                                    SphereOpenedTotal,
                                                    CapsuleOpenedTotal,
                                                    AttacksDone,
                                                    AttacksReceived,
                                                    CoinsSpentTotal,
                                                    TokensSpentTotal,
                                                    GiftsSent,
                                                    GiftsReceived,
                                                    MessagesSentTotal,
                                                    BragTotal,
                                                    PurchasedItemsTotal)
                          SELECT UserId,
                                 CoinsEarnedTotal,
                                 TokensEarnedTotal,
                                 ChestsEarnedTotal,
                                 SphereEarnedTotal,
                                 CapsuleEarnedTotal,
                                 ChestsOpenedTotal,
                                 SphereOpenedTotal,
                                 CapsuleOpenedTotal,
                                 AttacksDone,
                                 AttacksReceived,
                                 CoinsSpentTotal,
                                 TokensSpentTotal,
                                 GiftsSent,
                                 GiftsReceived,
                                 MessagesSentTotal,
                                 BragTotal,
                                 PurchasedItemsTotal
                          FROM `Users`");

            builder.DropColumn(name: "CoinsEarnedTotal", table: "Users");
            builder.DropColumn(name: "TokensEarnedTotal", table: "Users");
            builder.DropColumn(name: "ChestsEarnedTotal", table: "Users");
            builder.DropColumn(name: "SphereEarnedTotal", table: "Users");
            builder.DropColumn(name: "CapsuleEarnedTotal", table: "Users");
            builder.DropColumn(name: "ChestsOpenedTotal", table: "Users");
            builder.DropColumn(name: "SphereOpenedTotal", table: "Users");
            builder.DropColumn(name: "CapsuleOpenedTotal", table: "Users");
            builder.DropColumn(name: "AttacksDone", table: "Users");
            builder.DropColumn(name: "AttacksReceived", table: "Users");
            builder.DropColumn(name: "CoinsSpentTotal", table: "Users");
            builder.DropColumn(name: "TokensSpentTotal", table: "Users");
            builder.DropColumn(name: "GiftsSent", table: "Users");
            builder.DropColumn(name: "GiftsReceived", table: "Users");
            builder.DropColumn(name: "MessagesSentTotal", table: "Users");
            builder.DropColumn(name: "BragTotal", table: "Users");
            builder.DropColumn(name: "PurchasedItemsTotal", table: "Users");

            builder.CreateTable(name: "TempRoles",
                                columns: table => new
                                {
                                    UserId = table.Column<ulong>(nullable: false)
                                },
                                constraints: table =>
                                {
                                    table.PrimaryKey("PK_TempRoles", x => x.UserId);
                                    table.ForeignKey(name: "FK_TempRoles_Users_UserId",
                                                     column: x => x.UserId,
                                                     principalTable: "Users",
                                                     principalColumn: "UserId",
                                                     onDelete: ReferentialAction.Cascade);
                                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Achievements");

            migrationBuilder.DropTable(name: "Cooldowns");

            migrationBuilder.DropTable(name: "Inventory");

            migrationBuilder.DropTable(name: "LolData");

            migrationBuilder.DropTable(name: "PendingUsers");

            migrationBuilder.DropTable(name: "ScheduledEvents");

            migrationBuilder.DropTable(name: "Statistics");

            migrationBuilder.DropTable(name: "TempRoles");

            migrationBuilder.AddColumn<bool>(name: "Attack200times",
                                             table: "Users",
                                             nullable: false,
                                             defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "AttackWise",
                                             table: "Users",
                                             nullable: false,
                                             defaultValue: false);

            migrationBuilder.AddColumn<ulong>(name: "AttacksDone",
                                              table: "Users",
                                              nullable: false,
                                              defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(name: "AttacksReceived",
                                              table: "Users",
                                              nullable: false,
                                              defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(name: "BotRespectTimestamp",
                                              table: "Users",
                                              nullable: false,
                                              defaultValue: 0ul);

            migrationBuilder.AddColumn<bool>(name: "Brag100times",
                                             table: "Users",
                                             nullable: false,
                                             defaultValue: false);

            migrationBuilder.AddColumn<ulong>(name: "BragTotal",
                                              table: "Users",
                                              nullable: false,
                                              defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(name: "CapsuleEarnedTotal",
                                              table: "Users",
                                              nullable: false,
                                              defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(name: "CapsuleOpenedTotal",
                                              table: "Users",
                                              nullable: false,
                                              defaultValue: 0ul);

            migrationBuilder.AddColumn<uint>(name: "Capsules",
                                             table: "Users",
                                             nullable: false,
                                             defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(name: "Chests",
                                             table: "Users",
                                             nullable: false,
                                             defaultValue: 0u);

            migrationBuilder.AddColumn<ulong>(name: "ChestsEarnedTotal",
                                              table: "Users",
                                              nullable: false,
                                              defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(name: "ChestsOpenedTotal",
                                              table: "Users",
                                              nullable: false,
                                              defaultValue: 0ul);

            migrationBuilder.AddColumn<uint>(name: "Coins",
                                             table: "Users",
                                             nullable: false,
                                             defaultValue: 0u);

            migrationBuilder.AddColumn<ulong>(name: "CoinsEarnedTotal",
                                              table: "Users",
                                              nullable: false,
                                              defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(name: "CoinsSpentTotal",
                                              table: "Users",
                                              nullable: false,
                                              defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(name: "CreatedAtTimestamp",
                                              table: "Users",
                                              nullable: false,
                                              defaultValue: 0ul);

            migrationBuilder.AddColumn<uint>(name: "CustomTickets",
                                             table: "Users",
                                             nullable: false,
                                             defaultValue: 0u);

            migrationBuilder.AddColumn<ulong>(name: "DoubleExpTimestamp",
                                              table: "Users",
                                              nullable: false,
                                              defaultValue: 0ul);

            migrationBuilder.AddColumn<bool>(name: "GiftSphere",
                                             table: "Users",
                                             nullable: false,
                                             defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "GiftToBotKeeper",
                                             table: "Users",
                                             nullable: false,
                                             defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "GiftToModerator",
                                             table: "Users",
                                             nullable: false,
                                             defaultValue: false);

            migrationBuilder.AddColumn<ulong>(name: "GiftsReceived",
                                              table: "Users",
                                              nullable: false,
                                              defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(name: "GiftsSent",
                                              table: "Users",
                                              nullable: false,
                                              defaultValue: 0ul);

            migrationBuilder.AddColumn<uint>(name: "GiveawayTickets",
                                             table: "Users",
                                             nullable: false,
                                             defaultValue: 0u);

            migrationBuilder.AddColumn<bool>(name: "HasDonatedRole",
                                             table: "Users",
                                             nullable: false,
                                             defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "IsDonater",
                                             table: "Users",
                                             nullable: false,
                                             defaultValue: false);

            migrationBuilder.AddColumn<ulong>(name: "LastAttackTimestamp",
                                              table: "Users",
                                              nullable: false,
                                              defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(name: "LastBragTimestamp",
                                              table: "Users",
                                              nullable: false,
                                              defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(name: "LastDailyChestTimestamp",
                                              table: "Users",
                                              nullable: false,
                                              defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(name: "LastGiftTimestamp",
                                              table: "Users",
                                              nullable: false,
                                              defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(name: "LastLolAccountUpdateTimestamp",
                                              table: "Users",
                                              nullable: false,
                                              defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(name: "LastStoreTimestamp",
                                              table: "Users",
                                              nullable: false,
                                              defaultValue: 0ul);

            migrationBuilder.AddColumn<long>(name: "LolAccountId",
                                             table: "Users",
                                             nullable: false,
                                             defaultValue: 0L);

            migrationBuilder.AddColumn<long>(name: "LolSummonerId",
                                             table: "Users",
                                             nullable: false,
                                             defaultValue: 0L);

            migrationBuilder.AddColumn<string>(name: "LolSummonerName",
                                               table: "Users",
                                               nullable: true);

            migrationBuilder.AddColumn<string>(name: "LolSummonerRegion",
                                               table: "Users",
                                               nullable: true);

            migrationBuilder.AddColumn<ulong>(name: "MessagesSentTotal",
                                              table: "Users",
                                              nullable: false,
                                              defaultValue: 0ul);

            migrationBuilder.AddColumn<bool>(name: "Open100Chests",
                                             table: "Users",
                                             nullable: false,
                                             defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "OpenSphere",
                                             table: "Users",
                                             nullable: false,
                                             defaultValue: false);

            migrationBuilder.AddColumn<uint>(name: "PowerupsAttack",
                                             table: "Users",
                                             nullable: false,
                                             defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(name: "PowerupsBotRespect",
                                             table: "Users",
                                             nullable: false,
                                             defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(name: "PowerupsDoubleExp",
                                             table: "Users",
                                             nullable: false,
                                             defaultValue: 0u);

            migrationBuilder.AddColumn<bool>(name: "Purchaise200Items",
                                             table: "Users",
                                             nullable: false,
                                             defaultValue: false);

            migrationBuilder.AddColumn<ulong>(name: "PurchasedItemsTotal",
                                              table: "Users",
                                              nullable: false,
                                              defaultValue: 0ul);

            migrationBuilder.AddColumn<bool>(name: "Reach10Level",
                                             table: "Users",
                                             nullable: false,
                                             defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "Reach30Level",
                                             table: "Users",
                                             nullable: false,
                                             defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "Send100Gifts",
                                             table: "Users",
                                             nullable: false,
                                             defaultValue: false);

            migrationBuilder.AddColumn<ulong>(name: "SphereEarnedTotal",
                                              table: "Users",
                                              nullable: false,
                                              defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(name: "SphereOpenedTotal",
                                              table: "Users",
                                              nullable: false,
                                              defaultValue: 0ul);

            migrationBuilder.AddColumn<uint>(name: "Spheres",
                                             table: "Users",
                                             nullable: false,
                                             defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(name: "Tokens",
                                             table: "Users",
                                             nullable: false,
                                             defaultValue: 0u);

            migrationBuilder.AddColumn<ulong>(name: "TokensEarnedTotal",
                                              table: "Users",
                                              nullable: false,
                                              defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(name: "TokensSpentTotal",
                                              table: "Users",
                                              nullable: false,
                                              defaultValue: 0ul);

            migrationBuilder.AddColumn<bool>(name: "Write1000Mesages",
                                             table: "Users",
                                             nullable: false,
                                             defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "Write100Mesages",
                                             table: "Users",
                                             nullable: false,
                                             defaultValue: false);

            migrationBuilder.CreateTable(name: "Events",
                                         columns: table => new
                                         {
                                             Id = table.Column<uint>(nullable: false)
                                                       .Annotation("MySql:ValueGenerationStrategy",
                                                                   MySqlValueGenerationStrategy.IdentityColumn),
                                             DayId = table.Column<int>(nullable: false),
                                             EventId = table.Column<int>(nullable: false),
                                             Hour = table.Column<int>(nullable: false),
                                             Minute = table.Column<int>(nullable: false)
                                         },
                                         constraints: table => { table.PrimaryKey("PK_Events", x => x.Id); });
        }
    }
}
