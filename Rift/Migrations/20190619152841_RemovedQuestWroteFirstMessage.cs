using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class RemovedQuestWroteFirstMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "WroteFirstMessage",
                "Quests");

            migrationBuilder.DropColumn(
                "WroteFirstMessage",
                "QuestProgress");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                "WroteFirstMessage",
                "Quests",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                "WroteFirstMessage",
                "QuestProgress",
                nullable: true);
        }
    }
}
