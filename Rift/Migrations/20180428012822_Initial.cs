using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
	public partial class Initial : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Users",
				columns: table => new
				{
					UserId = table.Column<ulong>(nullable: false)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
					Experience = table.Column<uint>(nullable: false),
					Level = table.Column<uint>(nullable: false),
					Coins = table.Column<uint>(nullable: false),
					Tokens = table.Column<uint>(nullable: false),
					Chests = table.Column<uint>(nullable: false),
					Capsules = table.Column<uint>(nullable: false),
					Donate = table.Column<decimal>(nullable: false),
					PowerupId = table.Column<uint>(nullable: false),
					LolSummonerRegion = table.Column<string>(nullable: true),
					LolAccountId = table.Column<long>(nullable: false),
					LolSummonerId = table.Column<long>(nullable: false),
					LolSummonerName = table.Column<string>(nullable: true),
					LastStoreTimestamp = table.Column<ulong>(nullable: false),
					LastAttackTimestamp = table.Column<ulong>(nullable: false),
					CreatedAtTimestamp = table.Column<ulong>(nullable: false),
					CoinsEarnedTotal = table.Column<uint>(nullable: false),
					CoinsSpentTotal = table.Column<uint>(nullable: false),
					GiftsSent = table.Column<uint>(nullable: false),
					GiftsReceived = table.Column<uint>(nullable: false),
					AttacksDone = table.Column<uint>(nullable: false),
					AttacksReceived = table.Column<uint>(nullable: false),
					HasBotRespect = table.Column<bool>(nullable: false),
					LastDailyChestTimestamp = table.Column<ulong>(nullable: false),
					LastBragTimestamp = table.Column<ulong>(nullable: false),
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Users", x => x.UserId);
				});
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Users");
		}
	}
}