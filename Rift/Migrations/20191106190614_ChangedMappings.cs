using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class ChangedMappings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Identifier",
                table: "Mappings",
                newName: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Mappings",
                newName: "Identifier");
        }
    }
}
