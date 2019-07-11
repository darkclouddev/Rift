using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class SetQuestConditionsSameType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "VoiceTimeEarned",
                "Quests");

            migrationBuilder.DropColumn(
                "OpenedSphere",
                "QuestProgress");

            migrationBuilder.DropColumn(
                "VoiceTimeEarned",
                "QuestProgress");

            migrationBuilder.AlterColumn<uint>(
                "WroteFirstMessage",
                "Quests",
                nullable: true,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "OpenedSphere",
                "Quests",
                nullable: true,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "GiftedStreamer",
                "Quests",
                nullable: true,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "GiftedModerator",
                "Quests",
                nullable: true,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "GiftedBotKeeper",
                "Quests",
                nullable: true,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "ApprovedLolAccount",
                "Quests",
                nullable: true,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AddColumn<uint>(
                "VoiceMinutesEarned",
                "Quests",
                nullable: true);

            migrationBuilder.AlterColumn<uint>(
                "WroteFirstMessage",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "GiftedStreamer",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "GiftedModerator",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "GiftedBotKeeper",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "ApprovedLolAccount",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AddColumn<uint>(
                "OpenedSpheres",
                "QuestProgress",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                "VoiceMinutesEarned",
                "QuestProgress",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "VoiceMinutesEarned",
                "Quests");

            migrationBuilder.DropColumn(
                "OpenedSpheres",
                "QuestProgress");

            migrationBuilder.DropColumn(
                "VoiceMinutesEarned",
                "QuestProgress");

            migrationBuilder.AlterColumn<bool>(
                "WroteFirstMessage",
                "Quests",
                nullable: true,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                "OpenedSphere",
                "Quests",
                nullable: true,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                "GiftedStreamer",
                "Quests",
                nullable: true,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                "GiftedModerator",
                "Quests",
                nullable: true,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                "GiftedBotKeeper",
                "Quests",
                nullable: true,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                "ApprovedLolAccount",
                "Quests",
                nullable: true,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                "VoiceTimeEarned",
                "Quests",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                "WroteFirstMessage",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                "GiftedStreamer",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                "GiftedModerator",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                "GiftedBotKeeper",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                "ApprovedLolAccount",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                "OpenedSphere",
                "QuestProgress",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                "VoiceTimeEarned",
                "QuestProgress",
                nullable: true);
        }
    }
}
