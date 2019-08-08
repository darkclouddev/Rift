using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class AddedStreamerName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastDailyChestTime",
                table: "Cooldowns");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Streamers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Streamers");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastDailyChestTime",
                table: "Cooldowns",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
