using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class ChangedEventScheduleDateField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Month",
                table: "EventSchedule",
                newName: "EventType");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartAt",
                table: "EventSchedule",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartAt",
                table: "EventSchedule");

            migrationBuilder.RenameColumn(
                name: "EventType",
                table: "EventSchedule",
                newName: "Month");

            migrationBuilder.AddColumn<int>(
                name: "Day",
                table: "EventSchedule",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EventName",
                table: "EventSchedule",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Hour",
                table: "EventSchedule",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Minute",
                table: "EventSchedule",
                nullable: false,
                defaultValue: 0);
        }
    }
}
