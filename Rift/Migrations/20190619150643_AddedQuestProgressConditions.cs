using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class AddedQuestProgressConditions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                "RewardId",
                "Quests",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                "IsCompleted",
                "QuestProgress",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "RewardId",
                "Quests");

            migrationBuilder.DropColumn(
                "IsCompleted",
                "QuestProgress");
        }
    }
}
