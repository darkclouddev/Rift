using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class AddedALOT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayId",
                table: "ScheduledEvents");

            migrationBuilder.DropColumn(
                name: "Hour",
                table: "ScheduledEvents");

            migrationBuilder.DropColumn(
                name: "Minute",
                table: "ScheduledEvents");

            migrationBuilder.DropColumn(
                name: "VoiceMinutesEarned",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "OpenedSpheres",
                table: "QuestProgress");

            migrationBuilder.DropColumn(
                name: "VoiceMinutesEarned",
                table: "QuestProgress");

            migrationBuilder.RenameColumn(
                name: "TokensSpentTotal",
                table: "Statistics",
                newName: "TokensSpent");

            migrationBuilder.RenameColumn(
                name: "TokensEarnedTotal",
                table: "Statistics",
                newName: "TokensEarned");

            migrationBuilder.RenameColumn(
                name: "SphereOpenedTotal",
                table: "Statistics",
                newName: "TicketsSpent");

            migrationBuilder.RenameColumn(
                name: "SphereEarnedTotal",
                table: "Statistics",
                newName: "TicketsEarned");

            migrationBuilder.RenameColumn(
                name: "PurchasedItemsTotal",
                table: "Statistics",
                newName: "SpheresOpened");

            migrationBuilder.RenameColumn(
                name: "MessagesSentTotal",
                table: "Statistics",
                newName: "SpheresEarned");

            migrationBuilder.RenameColumn(
                name: "EssenceEarnedTotal",
                table: "Statistics",
                newName: "RewindsEarned");

            migrationBuilder.RenameColumn(
                name: "CoinsSpentTotal",
                table: "Statistics",
                newName: "RewindsActivated");

            migrationBuilder.RenameColumn(
                name: "CoinsEarnedTotal",
                table: "Statistics",
                newName: "PurchasedItems");

            migrationBuilder.RenameColumn(
                name: "ChestsOpenedTotal",
                table: "Statistics",
                newName: "MessagesSent");

            migrationBuilder.RenameColumn(
                name: "ChestsEarnedTotal",
                table: "Statistics",
                newName: "EssenceSpent");

            migrationBuilder.RenameColumn(
                name: "CapsuleOpenedTotal",
                table: "Statistics",
                newName: "EssenceEarned");

            migrationBuilder.RenameColumn(
                name: "CapsuleEarnedTotal",
                table: "Statistics",
                newName: "DoubleExpsEarned");

            migrationBuilder.RenameColumn(
                name: "BragTotal",
                table: "Statistics",
                newName: "DoubleExpsActivated");

            migrationBuilder.RenameColumn(
                name: "AttacksReceived",
                table: "Statistics",
                newName: "CoinsSpent");

            migrationBuilder.RenameColumn(
                name: "AttacksDone",
                table: "Statistics",
                newName: "CoinsEarned");

            migrationBuilder.AddColumn<uint>(
                name: "BotRespectsActivated",
                table: "Statistics",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "BotRespectsEarned",
                table: "Statistics",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "BragsDone",
                table: "Statistics",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "CapsulesEarned",
                table: "Statistics",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "CapsulesOpened",
                table: "Statistics",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "ChestsEarned",
                table: "Statistics",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "ChestsOpened",
                table: "Statistics",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ScheduledEvents",
                nullable: false,
                oldClrType: typeof(uint))
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "ScheduledEvents",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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
                name: "VoiceUptimeEarned",
                table: "Quests",
                nullable: true);

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
                name: "VoiceUptimeEarned",
                table: "QuestProgress",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BotRespectsActivated",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "BotRespectsEarned",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "BragsDone",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "CapsulesEarned",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "CapsulesOpened",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "ChestsEarned",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "ChestsOpened",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "ScheduledEvents");

            migrationBuilder.DropColumn(
                name: "VoiceUptimeEarned",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "OpenedSphere",
                table: "QuestProgress");

            migrationBuilder.DropColumn(
                name: "VoiceUptimeEarned",
                table: "QuestProgress");

            migrationBuilder.RenameColumn(
                name: "TokensSpent",
                table: "Statistics",
                newName: "TokensSpentTotal");

            migrationBuilder.RenameColumn(
                name: "TokensEarned",
                table: "Statistics",
                newName: "TokensEarnedTotal");

            migrationBuilder.RenameColumn(
                name: "TicketsSpent",
                table: "Statistics",
                newName: "SphereOpenedTotal");

            migrationBuilder.RenameColumn(
                name: "TicketsEarned",
                table: "Statistics",
                newName: "SphereEarnedTotal");

            migrationBuilder.RenameColumn(
                name: "SpheresOpened",
                table: "Statistics",
                newName: "PurchasedItemsTotal");

            migrationBuilder.RenameColumn(
                name: "SpheresEarned",
                table: "Statistics",
                newName: "MessagesSentTotal");

            migrationBuilder.RenameColumn(
                name: "RewindsEarned",
                table: "Statistics",
                newName: "EssenceEarnedTotal");

            migrationBuilder.RenameColumn(
                name: "RewindsActivated",
                table: "Statistics",
                newName: "CoinsSpentTotal");

            migrationBuilder.RenameColumn(
                name: "PurchasedItems",
                table: "Statistics",
                newName: "CoinsEarnedTotal");

            migrationBuilder.RenameColumn(
                name: "MessagesSent",
                table: "Statistics",
                newName: "ChestsOpenedTotal");

            migrationBuilder.RenameColumn(
                name: "EssenceSpent",
                table: "Statistics",
                newName: "ChestsEarnedTotal");

            migrationBuilder.RenameColumn(
                name: "EssenceEarned",
                table: "Statistics",
                newName: "CapsuleOpenedTotal");

            migrationBuilder.RenameColumn(
                name: "DoubleExpsEarned",
                table: "Statistics",
                newName: "CapsuleEarnedTotal");

            migrationBuilder.RenameColumn(
                name: "DoubleExpsActivated",
                table: "Statistics",
                newName: "BragTotal");

            migrationBuilder.RenameColumn(
                name: "CoinsSpent",
                table: "Statistics",
                newName: "AttacksReceived");

            migrationBuilder.RenameColumn(
                name: "CoinsEarned",
                table: "Statistics",
                newName: "AttacksDone");

            migrationBuilder.AlterColumn<uint>(
                name: "Id",
                table: "ScheduledEvents",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "DayId",
                table: "ScheduledEvents",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Hour",
                table: "ScheduledEvents",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Minute",
                table: "ScheduledEvents",
                nullable: false,
                defaultValue: 0);

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
    }
}
