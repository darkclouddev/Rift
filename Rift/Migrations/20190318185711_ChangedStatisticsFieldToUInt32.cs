using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class ChangedStatisticsFieldToUInt32 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<uint>(
                name: "TokensSpentTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<uint>(
                name: "TokensEarnedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<uint>(
                name: "SphereOpenedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<uint>(
                name: "SphereEarnedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<uint>(
                name: "PurchasedItemsTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<uint>(
                name: "MessagesSentTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<uint>(
                name: "GiftsSent",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<uint>(
                name: "GiftsReceived",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<uint>(
                name: "CoinsSpentTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<uint>(
                name: "CoinsEarnedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<uint>(
                name: "ChestsOpenedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<uint>(
                name: "ChestsEarnedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<uint>(
                name: "CapsuleOpenedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<uint>(
                name: "CapsuleEarnedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<uint>(
                name: "BragTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<uint>(
                name: "AttacksReceived",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<uint>(
                name: "AttacksDone",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(ulong));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ulong>(
                name: "TokensSpentTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<ulong>(
                name: "TokensEarnedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<ulong>(
                name: "SphereOpenedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<ulong>(
                name: "SphereEarnedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<ulong>(
                name: "PurchasedItemsTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<ulong>(
                name: "MessagesSentTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<ulong>(
                name: "GiftsSent",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<ulong>(
                name: "GiftsReceived",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<ulong>(
                name: "CoinsSpentTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<ulong>(
                name: "CoinsEarnedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<ulong>(
                name: "ChestsOpenedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<ulong>(
                name: "ChestsEarnedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<ulong>(
                name: "CapsuleOpenedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<ulong>(
                name: "CapsuleEarnedTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<ulong>(
                name: "BragTotal",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<ulong>(
                name: "AttacksReceived",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<ulong>(
                name: "AttacksDone",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(uint));
        }
    }
}
