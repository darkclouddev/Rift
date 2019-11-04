using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class ChangedVoiceUptimeFieldType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "VoiceUptimeHours",
                table: "Statistics",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.Sql(@"UPDATE Statistics SET VoiceUptimeHours = HOUR(VoiceUptime);");
            
            migrationBuilder.DropColumn(
                name: "VoiceUptime",
                table: "Statistics");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoiceUptimeHours",
                table: "Statistics");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "VoiceUptime",
                table: "Statistics",
                type: "time(6)",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }
    }
}
