using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
	public partial class AddedLastGiftTimestamp : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<ulong>(
				name: "LastGiftTimestamp",
				table: "Users",
				nullable: false,
				defaultValue: 0ul);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "LastGiftTimestamp",
				table: "Users");
		}
	}
}