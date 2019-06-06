using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class RemovedToxicityLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                table: "Toxicity");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "Level",
                table: "Toxicity",
                nullable: false,
                defaultValue: 0u);
        }
    }
}
