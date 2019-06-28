using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class RemovedQuestWroteFirstMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WroteFirstMessage",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "WroteFirstMessage",
                table: "QuestProgress");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "WroteFirstMessage",
                table: "Quests",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "WroteFirstMessage",
                table: "QuestProgress",
                nullable: true);
        }
    }
}
