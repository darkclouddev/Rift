using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class ChangedGiftFounderFieldType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GiftsReceivedFromUltraGay",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "GiftsReceivedFromUltraGay",
                table: "QuestProgress");

            migrationBuilder.AddColumn<bool>(
                name: "GiftedFounder",
                table: "Quests",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "GiftedFounder",
                table: "QuestProgress",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GiftedFounder",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "GiftedFounder",
                table: "QuestProgress");

            migrationBuilder.AddColumn<uint>(
                name: "GiftsReceivedFromUltraGay",
                table: "Quests",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "GiftsReceivedFromUltraGay",
                table: "QuestProgress",
                nullable: true);
        }
    }
}
