using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class ChangedInventory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                "UsualTickets",
                "Inventory",
                "Tickets");

            migrationBuilder.RenameColumn(
                "RareTickets",
                "Inventory",
                "Essence");

            migrationBuilder.RenameColumn(
                "PowerupsDoubleExp",
                "Inventory",
                "BonusRewind");

            migrationBuilder.RenameColumn(
                "PowerupsBotRespect",
                "Inventory",
                "BonusDoubleExp");

            migrationBuilder.AddColumn<uint>(
                "BonusBotRespect",
                "Inventory",
                nullable: false,
                defaultValue: 0u);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "BonusBotRespect",
                "Inventory");

            migrationBuilder.RenameColumn(
                "Tickets",
                "Inventory",
                "UsualTickets");

            migrationBuilder.RenameColumn(
                "Essence",
                "Inventory",
                "RareTickets");

            migrationBuilder.RenameColumn(
                "BonusRewind",
                "Inventory",
                "PowerupsDoubleExp");

            migrationBuilder.RenameColumn(
                "BonusDoubleExp",
                "Inventory",
                "PowerupsBotRespect");
        }
    }
}
