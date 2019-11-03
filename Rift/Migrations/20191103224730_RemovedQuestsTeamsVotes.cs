using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class RemovedQuestsTeamsVotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Communities");

            migrationBuilder.DropTable(
                name: "QuestProgress");

            migrationBuilder.DropTable(
                name: "Quests");

            migrationBuilder.DropTable(
                name: "QuestStages");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Votes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Communities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BackgroundId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: true),
                    Url = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Communities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestProgress",
                columns: table => new
                {
                    UserId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    QuestId = table.Column<int>(type: "int", nullable: false),
                    ActivatedBotRespects = table.Column<uint>(type: "int unsigned", nullable: true),
                    ApprovedLolAccount = table.Column<bool>(type: "bit", nullable: true),
                    BoughtChests = table.Column<uint>(type: "int unsigned", nullable: true),
                    BragsDone = table.Column<uint>(type: "int unsigned", nullable: true),
                    CoinsReceived = table.Column<uint>(type: "int unsigned", nullable: true),
                    CoinsSpent = table.Column<uint>(type: "int unsigned", nullable: true),
                    EpicMonstersKilled = table.Column<uint>(type: "int unsigned", nullable: true),
                    GiftedDeveloper = table.Column<bool>(type: "bit", nullable: true),
                    GiftedFounder = table.Column<bool>(type: "bit", nullable: true),
                    GiftedModerator = table.Column<bool>(type: "bit", nullable: true),
                    GiftedStreamer = table.Column<bool>(type: "bit", nullable: true),
                    GiftsReceived = table.Column<uint>(type: "int unsigned", nullable: true),
                    GiftsSent = table.Column<uint>(type: "int unsigned", nullable: true),
                    GiveawaysParticipated = table.Column<uint>(type: "int unsigned", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    LevelReached = table.Column<uint>(type: "int unsigned", nullable: true),
                    MessagesSent = table.Column<uint>(type: "int unsigned", nullable: true),
                    NormalMonstersKilled = table.Column<uint>(type: "int unsigned", nullable: true),
                    OpenedChests = table.Column<uint>(type: "int unsigned", nullable: true),
                    OpenedSphere = table.Column<bool>(type: "bit", nullable: true),
                    RareMonstersKilled = table.Column<uint>(type: "int unsigned", nullable: true),
                    RolesPurchased = table.Column<uint>(type: "int unsigned", nullable: true),
                    VoiceUptimeEarned = table.Column<TimeSpan>(type: "time(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestProgress", x => new { x.UserId, x.QuestId });
                });

            migrationBuilder.CreateTable(
                name: "Quests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ActivatedBotRespects = table.Column<uint>(type: "int unsigned", nullable: true),
                    ApprovedLolAccount = table.Column<bool>(type: "bit", nullable: true),
                    BoughtChests = table.Column<uint>(type: "int unsigned", nullable: true),
                    BragsDone = table.Column<uint>(type: "int unsigned", nullable: true),
                    CoinsReceived = table.Column<uint>(type: "int unsigned", nullable: true),
                    CoinsSpent = table.Column<uint>(type: "int unsigned", nullable: true),
                    EpicMonstersKilled = table.Column<uint>(type: "int unsigned", nullable: true),
                    GiftedDeveloper = table.Column<bool>(type: "bit", nullable: true),
                    GiftedFounder = table.Column<bool>(type: "bit", nullable: true),
                    GiftedModerator = table.Column<bool>(type: "bit", nullable: true),
                    GiftedStreamer = table.Column<bool>(type: "bit", nullable: true),
                    GiftsReceived = table.Column<uint>(type: "int unsigned", nullable: true),
                    GiftsSent = table.Column<uint>(type: "int unsigned", nullable: true),
                    GiveawaysParticipated = table.Column<uint>(type: "int unsigned", nullable: true),
                    LevelReached = table.Column<uint>(type: "int unsigned", nullable: true),
                    MessagesSent = table.Column<uint>(type: "int unsigned", nullable: true),
                    Name = table.Column<string>(type: "longtext", nullable: true),
                    NormalMonstersKilled = table.Column<uint>(type: "int unsigned", nullable: true),
                    OpenedChests = table.Column<uint>(type: "int unsigned", nullable: true),
                    OpenedSphere = table.Column<bool>(type: "bit", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    RareMonstersKilled = table.Column<uint>(type: "int unsigned", nullable: true),
                    RewardId = table.Column<int>(type: "int", nullable: false),
                    RolesPurchased = table.Column<uint>(type: "int unsigned", nullable: true),
                    StageId = table.Column<int>(type: "int", nullable: false),
                    VoiceUptimeEarned = table.Column<TimeSpan>(type: "time(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestStages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CompletionRewardId = table.Column<int>(type: "int", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestStages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BackgroundId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: true),
                    Url = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Votes",
                columns: table => new
                {
                    UserId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CommunityId = table.Column<int>(type: "int", nullable: false),
                    StreamerId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    TeamId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Votes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
