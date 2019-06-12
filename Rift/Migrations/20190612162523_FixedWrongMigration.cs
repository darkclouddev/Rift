using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class FixedWrongMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rewards_Giveaways_GiveawayName",
                table: "Rewards");

            migrationBuilder.DropIndex(
                name: "IX_Rewards_GiveawayName",
                table: "Rewards");

            migrationBuilder.DropColumn(
                name: "GiveawayName",
                table: "Rewards");

            migrationBuilder.RenameColumn(
                name: "MessageId",
                table: "Giveaways",
                newName: "StoredMessageId");

            migrationBuilder.RenameColumn(
                name: "MessageId",
                table: "ActiveGiveaways",
                newName: "ChannelMessageId");

            migrationBuilder.RenameColumn(
                name: "GiveawayId",
                table: "ActiveGiveaways",
                newName: "StoredMessageId");

            migrationBuilder.AddColumn<string>(
                name: "GiveawayName",
                table: "ActiveGiveaways",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GiveawayName",
                table: "ActiveGiveaways");

            migrationBuilder.RenameColumn(
                name: "StoredMessageId",
                table: "Giveaways",
                newName: "MessageId");

            migrationBuilder.RenameColumn(
                name: "StoredMessageId",
                table: "ActiveGiveaways",
                newName: "GiveawayId");

            migrationBuilder.RenameColumn(
                name: "ChannelMessageId",
                table: "ActiveGiveaways",
                newName: "MessageId");

            migrationBuilder.AddColumn<string>(
                name: "GiveawayName",
                table: "Rewards",
                nullable: true);

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
    }
}
