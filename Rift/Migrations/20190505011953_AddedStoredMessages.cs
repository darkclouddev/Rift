using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class AddedStoredMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessageMappings",
                columns: table => new
                {
                    Identifier = table.Column<string>(nullable: false, maxLength: 128),
                    MessageId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageMappings", x => x.Identifier);
                });

            migrationBuilder.CreateTable(
                name: "StoredMessages",
                columns: table => new
                {
                    MessageId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MessageName = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    Embed = table.Column<string>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true),
                    ApplyFormat = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredMessages", x => x.MessageId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageMappings");

            migrationBuilder.DropTable(
                name: "StoredMessages");
        }
    }
}
