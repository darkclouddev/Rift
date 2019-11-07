using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class ImplementedMessageApi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Embed",
                newName: "EmbedJson",
                table: "Messages");

            migrationBuilder.AddColumn<ulong>(
                name: "GuildId",
                table: "Messages",
                nullable: false,
                defaultValue: 213672490491314176ul);

            migrationBuilder.AddColumn<Guid>(
                name: "NewId",
                table: "Messages",
                nullable: false,
                defaultValue: new Guid());

            migrationBuilder.AddColumn<Guid>(
                name: "StoredMessageId",
                table: "MessageMappings",
                nullable: false,
                defaultValue: new Guid());

            migrationBuilder.AddColumn<Guid>(
                name: "MessageId",
                table: "Giveaways",
                nullable: false,
                defaultValue: new Guid());

            migrationBuilder.AddColumn<Guid>(
                name: "MessageId",
                table: "Events",
                nullable: false,
                defaultValue: new Guid());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuildId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "NewId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "StoredMessageId",
                table: "MessageMappings");

            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "Giveaways");

            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "Events");
            
            migrationBuilder.RenameColumn(
                name: "EmbedJson",
                newName: "Embed",
                table: "Messages");
        }
    }
}
