using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class AddedQuests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Quests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    StageId = table.Column<int>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    ApprovedLolAccount = table.Column<bool>(nullable: false),
                    WroteFirstMessage = table.Column<bool>(nullable: false),
                    BragsDone = table.Column<uint>(nullable: false),
                    MessagesSent = table.Column<uint>(nullable: false),
                    BoughtChests = table.Column<uint>(nullable: false),
                    OpenedChests = table.Column<uint>(nullable: false),
                    UsualMonstersKilled = table.Column<uint>(nullable: false),
                    RareMonstersKilled = table.Column<uint>(nullable: false),
                    EpicMonstersKilled = table.Column<uint>(nullable: false),
                    GiftsSent = table.Column<uint>(nullable: false),
                    GiftsReceived = table.Column<uint>(nullable: false),
                    GiftsReceivedFromUltraGay = table.Column<uint>(nullable: false),
                    LevelReached = table.Column<uint>(nullable: false),
                    GiveawaysParticipated = table.Column<uint>(nullable: false),
                    CoinsReceived = table.Column<uint>(nullable: false),
                    CoinsSpent = table.Column<uint>(nullable: false),
                    VoiceTimeEarned = table.Column<TimeSpan>(nullable: false),
                    GiftedBotKeeper = table.Column<bool>(nullable: false),
                    GiftedModerator = table.Column<bool>(nullable: false),
                    GiftedStreamer = table.Column<bool>(nullable: false),
                    ActivatedBotRespects = table.Column<uint>(nullable: false),
                    OpenedSphere = table.Column<bool>(nullable: false),
                    RolesPurchased = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestStages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    CompletionRewardId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestStages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserQuests",
                columns: table => new
                {
                    UserId = table.Column<ulong>(nullable: false),
                    QuestId = table.Column<int>(nullable: false),
                    ApprovedLolAccount = table.Column<bool>(nullable: false),
                    WroteFirstMessage = table.Column<bool>(nullable: false),
                    BragsDone = table.Column<uint>(nullable: false),
                    MessagesSent = table.Column<uint>(nullable: false),
                    BoughtChests = table.Column<uint>(nullable: false),
                    OpenedChests = table.Column<uint>(nullable: false),
                    UsualMonstersKilled = table.Column<uint>(nullable: false),
                    RareMonstersKilled = table.Column<uint>(nullable: false),
                    EpicMonstersKilled = table.Column<uint>(nullable: false),
                    GiftsSent = table.Column<uint>(nullable: false),
                    GiftsReceived = table.Column<uint>(nullable: false),
                    GiftsReceivedFromUltraGay = table.Column<uint>(nullable: false),
                    LevelReached = table.Column<uint>(nullable: false),
                    GiveawaysParticipated = table.Column<uint>(nullable: false),
                    CoinsReceived = table.Column<uint>(nullable: false),
                    CoinsSpent = table.Column<uint>(nullable: false),
                    VoiceTimeEarned = table.Column<TimeSpan>(nullable: false),
                    GiftedBotKeeper = table.Column<bool>(nullable: false),
                    GiftedModerator = table.Column<bool>(nullable: false),
                    GiftedStreamer = table.Column<bool>(nullable: false),
                    ActivatedBotRespects = table.Column<uint>(nullable: false),
                    OpenedSphere = table.Column<bool>(nullable: false),
                    RolesPurchased = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserQuests", x => new { x.UserId, x.QuestId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Quests");

            migrationBuilder.DropTable(
                name: "QuestStages");

            migrationBuilder.DropTable(
                name: "UserQuests");
        }
    }
}
