using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class ChangedSelectedRoleFieldType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SelectedRole",
                table: "Users",
                nullable: false,
                oldClrType: typeof(ulong),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ulong>(
                name: "SelectedRole",
                table: "Users",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
