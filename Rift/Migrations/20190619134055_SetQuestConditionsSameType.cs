using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class SetQuestConditionsSameType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoiceTimeEarned",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "OpenedSphere",
                table: "QuestProgress");

            migrationBuilder.DropColumn(
                name: "VoiceTimeEarned",
                table: "QuestProgress");

            migrationBuilder.AlterColumn<uint>(
                name: "WroteFirstMessage",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "OpenedSphere",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "GiftedStreamer",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "GiftedModerator",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "GiftedBotKeeper",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "ApprovedLolAccount",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "VoiceMinutesEarned",
                table: "Quests",
                nullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "WroteFirstMessage",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "GiftedStreamer",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "GiftedModerator",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "GiftedBotKeeper",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "ApprovedLolAccount",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "OpenedSpheres",
                table: "QuestProgress",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "VoiceMinutesEarned",
                table: "QuestProgress",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoiceMinutesEarned",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "OpenedSpheres",
                table: "QuestProgress");

            migrationBuilder.DropColumn(
                name: "VoiceMinutesEarned",
                table: "QuestProgress");

            migrationBuilder.AlterColumn<bool>(
                name: "WroteFirstMessage",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "OpenedSphere",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "GiftedStreamer",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "GiftedModerator",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "GiftedBotKeeper",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "ApprovedLolAccount",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "VoiceTimeEarned",
                table: "Quests",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "WroteFirstMessage",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "GiftedStreamer",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "GiftedModerator",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "GiftedBotKeeper",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "ApprovedLolAccount",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OpenedSphere",
                table: "QuestProgress",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "VoiceTimeEarned",
                table: "QuestProgress",
                nullable: true);
        }
    }
}
