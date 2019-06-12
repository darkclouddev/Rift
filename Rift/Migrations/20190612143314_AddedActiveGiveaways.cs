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
                name: "FK_Giveaways_Rewards_RewardId",
                table: "Giveaways");

            migrationBuilder.DropIndex(
                name: "IX_Giveaways_RewardId",
                table: "Giveaways");

            migrationBuilder.AddColumn<string>(
                name: "GiveawayName",
                table: "Rewards",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MessageId",
                table: "Giveaways",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ActiveGiveaways",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GiveawayId = table.Column<int>(nullable: false),
                    MessageId = table.Column<ulong>(nullable: false),
                    StartedBy = table.Column<ulong>(nullable: false),
                    StartedAt = table.Column<DateTime>(nullable: false),
                    DueTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveGiveaways", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rewards_GiveawayName",
                table: "Rewards",
                column: "GiveawayName");

            migrationBuilder.AddForeignKey(
                name: "FK_Rewards_Giveaways_GiveawayName",
                table: "Rewards",
                column: "GiveawayName",
                principalTable: "Giveaways",
                principalColumn: "Name",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rewards_Giveaways_GiveawayName",
                table: "Rewards");

            migrationBuilder.DropTable(
                name: "ActiveGiveaways");

            migrationBuilder.DropIndex(
                name: "IX_Rewards_GiveawayName",
                table: "Rewards");

            migrationBuilder.DropColumn(
                name: "GiveawayName",
                table: "Rewards");

            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "Giveaways");

            migrationBuilder.CreateIndex(
                name: "IX_Giveaways_RewardId",
                table: "Giveaways",
                column: "RewardId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Giveaways_Rewards_RewardId",
                table: "Giveaways",
                column: "RewardId",
                principalTable: "Rewards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
