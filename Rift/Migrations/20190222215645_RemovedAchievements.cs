using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class RemovedAchievements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Achievements");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Achievements",
                columns: table => new
                {
                    UserId = table.Column<ulong>(nullable: false),
                    Attack200Times = table.Column<bool>(nullable: false),
                    AttackWise = table.Column<bool>(nullable: false),
                    Brag100Times = table.Column<bool>(nullable: false),
                    GiftToBotKeeper = table.Column<bool>(nullable: false),
                    GiftToModerator = table.Column<bool>(nullable: false),
                    HasDonatedRole = table.Column<bool>(nullable: false),
                    IsDonator = table.Column<bool>(nullable: false),
                    Open100Chests = table.Column<bool>(nullable: false),
                    OpenSphere = table.Column<bool>(nullable: false),
                    Purchase200Items = table.Column<bool>(nullable: false),
                    Reach10Level = table.Column<bool>(nullable: false),
                    Reach30Level = table.Column<bool>(nullable: false),
                    Send100Gifts = table.Column<bool>(nullable: false),
                    Write1000Messages = table.Column<bool>(nullable: false),
                    Write100Messages = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Achievements_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
