using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class AddedBackgroundInventory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                "ProfileBackground",
                "Users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<ulong>(
                "SelectedRole",
                "Users",
                nullable: true);

            migrationBuilder.CreateTable(
                "BackgroundInventories",
                table => new
                {
                    UserId = table.Column<ulong>(nullable: false),
                    BackgroundId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackgroundInventories", x => new {x.UserId, x.BackgroundId});
                    table.ForeignKey(
                        "FK_BackgroundInventories_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "ProfileBackgrounds",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                              .Annotation("MySql:ValueGenerationStrategy",
                                          MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_ProfileBackgrounds", x => x.Id); });

            migrationBuilder.CreateIndex(
                "IX_BackgroundInventories_UserId",
                "BackgroundInventories",
                "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "BackgroundInventories");

            migrationBuilder.DropTable(
                "ProfileBackgrounds");

            migrationBuilder.DropColumn(
                "ProfileBackground",
                "Users");

            migrationBuilder.DropColumn(
                "SelectedRole",
                "Users");
        }
    }
}
