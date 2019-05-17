using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class RenamedMessageColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MessageName",
                table: "StoredMessages",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "MessageId",
                table: "StoredMessages",
                newName: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "StoredMessages",
                newName: "MessageName");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "StoredMessages",
                newName: "MessageId");
        }
    }
}
