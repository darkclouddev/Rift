using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class RenamedStoreCooldown : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastStoreTime",
                table: "Cooldowns",
                newName: "LastItemStoreTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastItemStoreTime",
                table: "Cooldowns",
                newName: "LastStoreTime");
        }
    }
}
