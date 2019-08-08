using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class FixedWrongMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_Rewards_Giveaways_GiveawayName",
                "Rewards");

            migrationBuilder.DropIndex(
                "IX_Rewards_GiveawayName",
                "Rewards");

            migrationBuilder.DropColumn(
                "GiveawayName",
                "Rewards");

            migrationBuilder.RenameColumn(
                "MessageId",
                "Giveaways",
                "StoredMessageId");

            migrationBuilder.RenameColumn(
                "MessageId",
                "ActiveGiveaways",
                "ChannelMessageId");

            migrationBuilder.RenameColumn(
                "GiveawayId",
                "ActiveGiveaways",
                "StoredMessageId");

            migrationBuilder.AddColumn<string>(
                "GiveawayName",
                "ActiveGiveaways",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "GiveawayName",
                "ActiveGiveaways");

            migrationBuilder.RenameColumn(
                "StoredMessageId",
                "Giveaways",
                "MessageId");

            migrationBuilder.RenameColumn(
                "StoredMessageId",
                "ActiveGiveaways",
                "GiveawayId");

            migrationBuilder.RenameColumn(
                "ChannelMessageId",
                "ActiveGiveaways",
                "MessageId");

            migrationBuilder.AddColumn<string>(
                "GiveawayName",
                "Rewards",
                nullable: true);

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
    }
}
