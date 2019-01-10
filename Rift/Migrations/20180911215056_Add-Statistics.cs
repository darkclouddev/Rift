using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Rift.Migrations
{
    public partial class AddStatistics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ulong>(
                name: "GiftsSent",
                table: "Users",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<ulong>(
                name: "GiftsReceived",
                table: "Users",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<ulong>(
                name: "CoinsSpentTotal",
                table: "Users",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<ulong>(
                name: "CoinsEarnedTotal",
                table: "Users",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<ulong>(
                name: "AttacksReceived",
                table: "Users",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<ulong>(
                name: "AttacksDone",
                table: "Users",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AddColumn<bool>(
                name: "Attack50times",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AttackWise",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Brag50times",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<ulong>(
                name: "BragTotal",
                table: "Users",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "CapsuleEarnedTotal",
                table: "Users",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "CapsuleOpenedTotal",
                table: "Users",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "ChestsEarnedTotal",
                table: "Users",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "ChestsOpenedTotal",
                table: "Users",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<bool>(
                name: "GiftSphere",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "GiftToBotKeeper",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "GiftToModerator",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasDonatedRole",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDonater",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<ulong>(
                name: "MessagesSentTotal",
                table: "Users",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<bool>(
                name: "Open100Chests",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OpenSphere",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Purchaise50Items",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<ulong>(
                name: "PurchasedItemsTotal",
                table: "Users",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<bool>(
                name: "Reach10Level",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Reach30Level",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Send50Gifts",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<ulong>(
                name: "SphereEarnedTotal",
                table: "Users",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "SphereOpenedTotal",
                table: "Users",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "TokensEarnedTotal",
                table: "Users",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<bool>(
                name: "Write1000Mesages",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Write100Mesages",
                table: "Users",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attack50times",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AttackWise",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Brag50times",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BragTotal",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CapsuleEarnedTotal",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CapsuleOpenedTotal",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ChestsEarnedTotal",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ChestsOpenedTotal",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GiftSphere",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GiftToBotKeeper",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GiftToModerator",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HasDonatedRole",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsDonater",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MessagesSentTotal",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Open100Chests",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OpenSphere",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Purchaise50Items",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PurchasedItemsTotal",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Reach10Level",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Reach30Level",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Send50Gifts",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SphereEarnedTotal",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SphereOpenedTotal",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TokensEarnedTotal",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Write1000Mesages",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Write100Mesages",
                table: "Users");

            migrationBuilder.AlterColumn<uint>(
                name: "GiftsSent",
                table: "Users",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<uint>(
                name: "GiftsReceived",
                table: "Users",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<uint>(
                name: "CoinsSpentTotal",
                table: "Users",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<uint>(
                name: "CoinsEarnedTotal",
                table: "Users",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<uint>(
                name: "AttacksReceived",
                table: "Users",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<uint>(
                name: "AttacksDone",
                table: "Users",
                nullable: false,
                oldClrType: typeof(ulong));
        }
    }
}
