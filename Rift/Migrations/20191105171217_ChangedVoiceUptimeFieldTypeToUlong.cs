using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class ChangedVoiceUptimeFieldTypeToUlong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "VoiceUptimeMinutes",
                table: "Statistics",
                nullable: false,
                defaultValue: 0ul);
            
            migrationBuilder.Sql(@"UPDATE Statistics SET VoiceUptimeMinutes = VoiceUptimeHours*60;");
            
            migrationBuilder.DropColumn(
                name: "VoiceUptimeHours",
                table: "Statistics");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoiceUptimeMinutes",
                table: "Statistics");
            
            migrationBuilder.Sql(@"UPDATE Statistics SET VoiceUptimeHours = VoiceUptimeMinutes/60;");

            migrationBuilder.AddColumn<uint>(
                name: "VoiceUptimeHours",
                table: "Statistics",
                type: "int unsigned",
                nullable: false,
                defaultValue: 0u);
        }
    }
}
