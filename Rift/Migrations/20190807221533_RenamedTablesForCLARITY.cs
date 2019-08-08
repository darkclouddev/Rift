using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class RenamedTablesForCLARITY : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LolData_Users_UserId",
                table: "LolData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoredMessages",
                table: "StoredMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProfileBackgrounds",
                table: "ProfileBackgrounds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LolData",
                table: "LolData");

            migrationBuilder.DropColumn(
                name: "ApplyFormat",
                table: "StoredMessages");

            migrationBuilder.RenameTable(
                name: "StoredMessages",
                newName: "Messages");

            migrationBuilder.RenameTable(
                name: "ProfileBackgrounds",
                newName: "Backgrounds");

            migrationBuilder.RenameTable(
                name: "LolData",
                newName: "LeagueData");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Messages",
                table: "Messages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Backgrounds",
                table: "Backgrounds",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeagueData",
                table: "LeagueData",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeagueData_Users_UserId",
                table: "LeagueData",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeagueData_Users_UserId",
                table: "LeagueData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Messages",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeagueData",
                table: "LeagueData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Backgrounds",
                table: "Backgrounds");

            migrationBuilder.RenameTable(
                name: "Messages",
                newName: "StoredMessages");

            migrationBuilder.RenameTable(
                name: "LeagueData",
                newName: "LolData");

            migrationBuilder.RenameTable(
                name: "Backgrounds",
                newName: "ProfileBackgrounds");

            migrationBuilder.AddColumn<bool>(
                name: "ApplyFormat",
                table: "StoredMessages",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoredMessages",
                table: "StoredMessages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LolData",
                table: "LolData",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProfileBackgrounds",
                table: "ProfileBackgrounds",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LolData_Users_UserId",
                table: "LolData",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
