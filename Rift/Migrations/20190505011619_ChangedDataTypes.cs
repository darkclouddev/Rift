using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class ChangedDataTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<uint>(
                name: "Level",
                table: "Users",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<uint>(
                name: "Experience",
                table: "Users",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<ulong>(
                name: "UserId",
                table: "Users",
                nullable: false,
                oldClrType: typeof(decimal))
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<ulong>(
                name: "RoleId",
                table: "TempRoles",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<ulong>(
                name: "UserId",
                table: "TempRoles",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<ulong>(
                name: "UserId",
                table: "Streamers",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<uint>(
                name: "TokensSpentTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<uint>(
                name: "TokensEarnedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<uint>(
                name: "SphereOpenedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<uint>(
                name: "SphereEarnedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<uint>(
                name: "PurchasedItemsTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<uint>(
                name: "MessagesSentTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<uint>(
                name: "GiftsSent",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<uint>(
                name: "GiftsReceived",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<uint>(
                name: "CoinsSpentTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<uint>(
                name: "CoinsEarnedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<uint>(
                name: "ChestsOpenedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<uint>(
                name: "ChestsEarnedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<uint>(
                name: "CapsuleOpenedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<uint>(
                name: "CapsuleEarnedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<uint>(
                name: "BragTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<uint>(
                name: "AttacksReceived",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<uint>(
                name: "AttacksDone",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<ulong>(
                name: "UserId",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<uint>(
                name: "Id",
                table: "ScheduledEvents",
                nullable: false,
                oldClrType: typeof(long))
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<ulong>(
                name: "UserId",
                table: "PendingUsers",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<ulong>(
                name: "UserId",
                table: "LolData",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<uint>(
                name: "UsualTickets",
                table: "Inventory",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<uint>(
                name: "Tokens",
                table: "Inventory",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<uint>(
                name: "Spheres",
                table: "Inventory",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<uint>(
                name: "RareTickets",
                table: "Inventory",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<uint>(
                name: "PowerupsDoubleExp",
                table: "Inventory",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<uint>(
                name: "PowerupsBotRespect",
                table: "Inventory",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<uint>(
                name: "Coins",
                table: "Inventory",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<uint>(
                name: "Chests",
                table: "Inventory",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<uint>(
                name: "Capsules",
                table: "Inventory",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<ulong>(
                name: "UserId",
                table: "Inventory",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<ulong>(
                name: "UserId",
                table: "Cooldowns",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Level",
                table: "Users",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<long>(
                name: "Experience",
                table: "Users",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<decimal>(
                name: "UserId",
                table: "Users",
                nullable: false,
                oldClrType: typeof(ulong))
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<decimal>(
                name: "RoleId",
                table: "TempRoles",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<decimal>(
                name: "UserId",
                table: "TempRoles",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<decimal>(
                name: "UserId",
                table: "Streamers",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<long>(
                name: "TokensSpentTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<long>(
                name: "TokensEarnedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<long>(
                name: "SphereOpenedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<long>(
                name: "SphereEarnedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<long>(
                name: "PurchasedItemsTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<long>(
                name: "MessagesSentTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<long>(
                name: "GiftsSent",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<long>(
                name: "GiftsReceived",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<long>(
                name: "CoinsSpentTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<long>(
                name: "CoinsEarnedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<long>(
                name: "ChestsOpenedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<long>(
                name: "ChestsEarnedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<long>(
                name: "CapsuleOpenedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<long>(
                name: "CapsuleEarnedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<long>(
                name: "BragTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<long>(
                name: "AttacksReceived",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<long>(
                name: "AttacksDone",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<decimal>(
                name: "UserId",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "ScheduledEvents",
                nullable: false,
                oldClrType: typeof(uint))
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<decimal>(
                name: "UserId",
                table: "PendingUsers",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<decimal>(
                name: "UserId",
                table: "LolData",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<long>(
                name: "UsualTickets",
                table: "Inventory",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<long>(
                name: "Tokens",
                table: "Inventory",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<long>(
                name: "Spheres",
                table: "Inventory",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<long>(
                name: "RareTickets",
                table: "Inventory",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<long>(
                name: "PowerupsDoubleExp",
                table: "Inventory",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<long>(
                name: "PowerupsBotRespect",
                table: "Inventory",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<long>(
                name: "Coins",
                table: "Inventory",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<long>(
                name: "Chests",
                table: "Inventory",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<long>(
                name: "Capsules",
                table: "Inventory",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<decimal>(
                name: "UserId",
                table: "Inventory",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<decimal>(
                name: "UserId",
                table: "Cooldowns",
                nullable: false,
                oldClrType: typeof(ulong));
        }
    }
}
