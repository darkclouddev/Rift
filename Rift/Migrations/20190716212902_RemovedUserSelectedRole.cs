using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class RemovedUserSelectedRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedRole",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "RewardId",
                table: "Quests",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SelectedRole",
                table: "Users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "RewardId",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
