using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class RenamedQuestField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UsualMonstersKilled",
                table: "Quests",
                newName: "NormalMonstersKilled");

            migrationBuilder.RenameColumn(
                name: "UsualMonstersKilled",
                table: "QuestProgress",
                newName: "NormalMonstersKilled");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Events",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "NormalMonstersKilled",
                table: "Quests",
                newName: "UsualMonstersKilled");

            migrationBuilder.RenameColumn(
                name: "NormalMonstersKilled",
                table: "QuestProgress",
                newName: "UsualMonstersKilled");
        }
    }
}
