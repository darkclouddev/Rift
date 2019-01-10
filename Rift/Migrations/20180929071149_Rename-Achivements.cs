using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Rift.Migrations
{
    public partial class RenameAchivements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Send50Gifts",
                table: "Users",
                newName: "Send100Gifts");

            migrationBuilder.RenameColumn(
                name: "Purchaise50Items",
                table: "Users",
                newName: "Purchaise200Items");

            migrationBuilder.RenameColumn(
                name: "Brag50times",
                table: "Users",
                newName: "Brag100times");

            migrationBuilder.RenameColumn(
                name: "Attack50times",
                table: "Users",
                newName: "Attack200times");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Send100Gifts",
                table: "Users",
                newName: "Send50Gifts");

            migrationBuilder.RenameColumn(
                name: "Purchaise200Items",
                table: "Users",
                newName: "Purchaise50Items");

            migrationBuilder.RenameColumn(
                name: "Brag100times",
                table: "Users",
                newName: "Brag50times");

            migrationBuilder.RenameColumn(
                name: "Attack200times",
                table: "Users",
                newName: "Attack50times");
        }
    }
}
