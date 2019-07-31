﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class AddedQuestStageDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "QuestStages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "QuestStages");
        }
    }
}
