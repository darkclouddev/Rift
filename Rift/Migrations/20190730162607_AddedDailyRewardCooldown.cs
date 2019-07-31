using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class AddedDailyRewardCooldown : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastDailyRewardTime",
                table: "Cooldowns",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastDailyRewardTime",
                table: "Cooldowns");
        }
    }
}
