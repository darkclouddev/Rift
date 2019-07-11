using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class RenamedStoreCooldown : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                "LastStoreTime",
                "Cooldowns",
                "LastItemStoreTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                "LastItemStoreTime",
                "Cooldowns",
                "LastStoreTime");
        }
    }
}
