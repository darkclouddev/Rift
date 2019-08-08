using System;

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class AddedSystemTimers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                "LastUpdated",
                "Toxicity",
                "LastIncreased");

            migrationBuilder.AddColumn<DateTime>(
                "LastDecreased",
                "Toxicity",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                "SystemCooldowns",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                              .Annotation("MySql:ValueGenerationStrategy",
                                          MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Interval = table.Column<TimeSpan>(nullable: false),
                    LastInvoked = table.Column<DateTime>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_SystemCooldowns", x => x.Id); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "SystemCooldowns");

            migrationBuilder.DropColumn(
                "LastDecreased",
                "Toxicity");

            migrationBuilder.RenameColumn(
                "LastIncreased",
                "Toxicity",
                "LastUpdated");
        }
    }
}
