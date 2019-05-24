using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class ChangedInventory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UsualTickets",
                table: "Inventory",
                newName: "Tickets");

            migrationBuilder.RenameColumn(
                name: "RareTickets",
                table: "Inventory",
                newName: "Essence");

            migrationBuilder.RenameColumn(
                name: "PowerupsDoubleExp",
                table: "Inventory",
                newName: "BonusRewind");

            migrationBuilder.RenameColumn(
                name: "PowerupsBotRespect",
                table: "Inventory",
                newName: "BonusDoubleExp");

            migrationBuilder.AddColumn<uint>(
                name: "BonusBotRespect",
                table: "Inventory",
                nullable: false,
                defaultValue: 0u);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BonusBotRespect",
                table: "Inventory");

            migrationBuilder.RenameColumn(
                name: "Tickets",
                table: "Inventory",
                newName: "UsualTickets");

            migrationBuilder.RenameColumn(
                name: "Essence",
                table: "Inventory",
                newName: "RareTickets");

            migrationBuilder.RenameColumn(
                name: "BonusRewind",
                table: "Inventory",
                newName: "PowerupsDoubleExp");

            migrationBuilder.RenameColumn(
                name: "BonusDoubleExp",
                table: "Inventory",
                newName: "PowerupsBotRespect");
        }
    }
}
