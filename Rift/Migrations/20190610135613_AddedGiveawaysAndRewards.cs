using System;

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class AddedGiveawaysAndRewards : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "GiveawayLogs",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                              .Annotation("MySql:ValueGenerationStrategy",
                                          MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    WinnersString = table.Column<string>(nullable: true),
                    ParticipantsString = table.Column<string>(nullable: true),
                    Reward = table.Column<string>(nullable: true),
                    StartedBy = table.Column<ulong>(nullable: false),
                    StartedAt = table.Column<DateTime>(nullable: false),
                    Duration = table.Column<TimeSpan>(nullable: false),
                    FinishedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_GiveawayLogs", x => x.Id); });

            migrationBuilder.CreateTable(
                "Rewards",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                              .Annotation("MySql:ValueGenerationStrategy",
                                          MySqlValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    ItemsData = table.Column<string>(nullable: true),
                    RoleData = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Rewards", x => x.Id); });

            migrationBuilder.CreateTable(
                "Giveaways",
                table => new
                {
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    WinnersAmount = table.Column<uint>(nullable: false),
                    RewardId = table.Column<int>(nullable: false),
                    Duration = table.Column<TimeSpan>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Giveaways", x => x.Name);
                    table.ForeignKey(
                        "FK_Giveaways_Rewards_RewardId",
                        x => x.RewardId,
                        "Rewards",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_Giveaways_RewardId",
                "Giveaways",
                "RewardId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "GiveawayLogs");

            migrationBuilder.DropTable(
                "Giveaways");

            migrationBuilder.DropTable(
                "Rewards");
        }
    }
}
