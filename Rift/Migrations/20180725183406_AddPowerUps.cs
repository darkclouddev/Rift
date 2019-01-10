using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Rift.Migrations
{
    public partial class AddPowerUps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasBotRespect",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "PowerupId",
                table: "Users",
                newName: "PowerupsProtection");

            migrationBuilder.AddColumn<uint>(
                name: "PowerupsAttack",
                table: "Users",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "PowerupsBotRespect",
                table: "Users",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "PowerupsDoubleExp",
                table: "Users",
                nullable: false,
                defaultValue: 0u);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PowerupsAttack",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PowerupsBotRespect",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PowerupsDoubleExp",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "PowerupsProtection",
                table: "Users",
                newName: "PowerupId");

            migrationBuilder.AddColumn<bool>(
                name: "HasBotRespect",
                table: "Users",
                nullable: false,
                defaultValue: false);
        }
    }
}
