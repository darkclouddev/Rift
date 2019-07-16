using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class ChangedRoleIdField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "RoleInventories",
                nullable: false,
                oldClrType: typeof(ulong));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ulong>(
                name: "RoleId",
                table: "RoleInventories",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
