using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class AddedBackgroundInventory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProfileBackground",
                table: "Users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<ulong>(
                name: "SelectedRole",
                table: "Users",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BackgroundInventories",
                columns: table => new
                {
                    UserId = table.Column<ulong>(nullable: false),
                    BackgroundId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackgroundInventories", x => new { x.UserId, x.BackgroundId });
                    table.ForeignKey(
                        name: "FK_BackgroundInventories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileBackgrounds",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileBackgrounds", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BackgroundInventories_UserId",
                table: "BackgroundInventories",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BackgroundInventories");

            migrationBuilder.DropTable(
                name: "ProfileBackgrounds");

            migrationBuilder.DropColumn(
                name: "ProfileBackground",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SelectedRole",
                table: "Users");
        }
    }
}
