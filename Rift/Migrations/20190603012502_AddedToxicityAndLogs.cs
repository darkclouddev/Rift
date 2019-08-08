using System;

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class AddedToxicityAndLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "ModerationLog",
                table => new
                {
                    Id = table.Column<uint>(nullable: false)
                              .Annotation("MySql:ValueGenerationStrategy",
                                          MySqlValueGenerationStrategy.IdentityColumn),
                    ModeratorId = table.Column<ulong>(nullable: false),
                    TargetId = table.Column<ulong>(nullable: false),
                    Action = table.Column<string>(nullable: true),
                    Reason = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Duration = table.Column<TimeSpan>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_ModerationLog", x => x.Id); });

            migrationBuilder.CreateTable(
                "Toxicity",
                table => new
                {
                    UserId = table.Column<ulong>(nullable: false),
                    Level = table.Column<uint>(nullable: false),
                    Percent = table.Column<uint>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Toxicity", x => x.UserId);
                    table.ForeignKey(
                        "FK_Toxicity_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "UserId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "ModerationLog");

            migrationBuilder.DropTable(
                "Toxicity");
        }
    }
}
