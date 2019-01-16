using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class AddedFluentApi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TempRoles_Users_UserId",
                table: "TempRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TempRoles",
                table: "TempRoles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TempRoles",
                table: "TempRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddForeignKey(
                name: "FK_RiftTempRoles_Users_UserId",
                table: "TempRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RiftTempRoles_Users_UserId",
                table: "TempRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TempRoles",
                table: "TempRoles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TempRoles",
                table: "TempRoles",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TempRoles_Users_UserId",
                table: "TempRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
