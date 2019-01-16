using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class FixedTempRoleModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationTime",
                table: "TempRoles",
                nullable: false,
                defaultValue: DateTime.MinValue);

            migrationBuilder.AddColumn<string>(
                name: "ObtainedFrom",
                table: "TempRoles",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ObtainedTime",
                table: "TempRoles",
                nullable: false,
                defaultValue: DateTime.MinValue);

            migrationBuilder.AddColumn<ulong>(
                name: "RoleId",
                table: "TempRoles",
                nullable: false,
                defaultValue: 0ul);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationTime",
                table: "TempRoles");

            migrationBuilder.DropColumn(
                name: "ObtainedFrom",
                table: "TempRoles");

            migrationBuilder.DropColumn(
                name: "ObtainedTime",
                table: "TempRoles");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "TempRoles");
        }
    }
}
