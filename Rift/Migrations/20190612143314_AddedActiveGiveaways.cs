using System;

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class AddedActiveGiveaways : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_Giveaways_Rewards_RewardId",
                "Giveaways");

            migrationBuilder.DropIndex(
                "IX_Giveaways_RewardId",
                "Giveaways");

            migrationBuilder.AddColumn<string>(
                "GiveawayName",
                "Rewards",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                "MessageId",
                "Giveaways",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                "ActiveGiveaways",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                              .Annotation("MySql:ValueGenerationStrategy",
                                          MySqlValueGenerationStrategy.IdentityColumn),
                    GiveawayId = table.Column<int>(nullable: false),
                    MessageId = table.Column<ulong>(nullable: false),
                    StartedBy = table.Column<ulong>(nullable: false),
                    StartedAt = table.Column<DateTime>(nullable: false),
                    DueTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_ActiveGiveaways", x => x.Id); });

            migrationBuilder.CreateIndex(
                "IX_Rewards_GiveawayName",
                "Rewards",
                "GiveawayName");

            migrationBuilder.AddForeignKey(
                "FK_Rewards_Giveaways_GiveawayName",
                "Rewards",
                "GiveawayName",
                "Giveaways",
                principalColumn: "Name",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_Rewards_Giveaways_GiveawayName",
                "Rewards");

            migrationBuilder.DropTable(
                "ActiveGiveaways");

            migrationBuilder.DropIndex(
                "IX_Rewards_GiveawayName",
                "Rewards");

            migrationBuilder.DropColumn(
                "GiveawayName",
                "Rewards");

            migrationBuilder.DropColumn(
                "MessageId",
                "Giveaways");

            migrationBuilder.CreateIndex(
                "IX_Giveaways_RewardId",
                "Giveaways",
                "RewardId",
                unique: true);

            migrationBuilder.AddForeignKey(
                "FK_Giveaways_Rewards_RewardId",
                "Giveaways",
                "RewardId",
                "Rewards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
