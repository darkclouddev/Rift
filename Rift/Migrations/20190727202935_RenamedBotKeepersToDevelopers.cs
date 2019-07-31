using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class RenamedBotKeepersToDevelopers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GiftedBotKeeper",
                table: "Quests",
                newName: "GiftedDeveloper");

            migrationBuilder.RenameColumn(
                name: "GiftedBotKeeper",
                table: "QuestProgress",
                newName: "GiftedDeveloper");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GiftedDeveloper",
                table: "Quests",
                newName: "GiftedBotKeeper");

            migrationBuilder.RenameColumn(
                name: "GiftedDeveloper",
                table: "QuestProgress",
                newName: "GiftedBotKeeper");
        }
    }
}
