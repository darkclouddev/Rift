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
                "DayId",
                "ScheduledEvents");

            migrationBuilder.DropColumn(
                "Hour",
                "ScheduledEvents");

            migrationBuilder.DropColumn(
                "Minute",
                "ScheduledEvents");

            migrationBuilder.DropColumn(
                "VoiceMinutesEarned",
                "Quests");

            migrationBuilder.DropColumn(
                "OpenedSpheres",
                "QuestProgress");

            migrationBuilder.DropColumn(
                "VoiceMinutesEarned",
                "QuestProgress");

            migrationBuilder.RenameColumn(
                "TokensSpentTotal",
                "Statistics",
                "TokensSpent");

            migrationBuilder.RenameColumn(
                "TokensEarnedTotal",
                "Statistics",
                "TokensEarned");

            migrationBuilder.RenameColumn(
                "SphereOpenedTotal",
                "Statistics",
                "TicketsSpent");

            migrationBuilder.RenameColumn(
                "SphereEarnedTotal",
                "Statistics",
                "TicketsEarned");

            migrationBuilder.RenameColumn(
                "PurchasedItemsTotal",
                "Statistics",
                "SpheresOpened");

            migrationBuilder.RenameColumn(
                "MessagesSentTotal",
                "Statistics",
                "SpheresEarned");

            migrationBuilder.RenameColumn(
                "EssenceEarnedTotal",
                "Statistics",
                "RewindsEarned");

            migrationBuilder.RenameColumn(
                "CoinsSpentTotal",
                "Statistics",
                "RewindsActivated");

            migrationBuilder.RenameColumn(
                "CoinsEarnedTotal",
                "Statistics",
                "PurchasedItems");

            migrationBuilder.RenameColumn(
                "ChestsOpenedTotal",
                "Statistics",
                "MessagesSent");

            migrationBuilder.RenameColumn(
                "ChestsEarnedTotal",
                "Statistics",
                "EssenceSpent");

            migrationBuilder.RenameColumn(
                "CapsuleOpenedTotal",
                "Statistics",
                "EssenceEarned");

            migrationBuilder.RenameColumn(
                "CapsuleEarnedTotal",
                "Statistics",
                "DoubleExpsEarned");

            migrationBuilder.RenameColumn(
                "BragTotal",
                "Statistics",
                "DoubleExpsActivated");

            migrationBuilder.RenameColumn(
                "AttacksReceived",
                "Statistics",
                "CoinsSpent");

            migrationBuilder.RenameColumn(
                "AttacksDone",
                "Statistics",
                "CoinsEarned");

            migrationBuilder.AddColumn<uint>(
                "BotRespectsActivated",
                "Statistics",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                "BotRespectsEarned",
                "Statistics",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                "BragsDone",
                "Statistics",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                "CapsulesEarned",
                "Statistics",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                "CapsulesOpened",
                "Statistics",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                "ChestsEarned",
                "Statistics",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                "ChestsOpened",
                "Statistics",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AlterColumn<int>(
                                "Id",
                                "ScheduledEvents",
                                nullable: false,
                                oldClrType: typeof(uint))
                            .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                            .OldAnnotation("MySql:ValueGenerationStrategy",
                                           MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                "Date",
                "ScheduledEvents",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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
                "VoiceUptimeEarned",
                "Quests",
                nullable: true);

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
                "VoiceUptimeEarned",
                "QuestProgress",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "BotRespectsActivated",
                "Statistics");

            migrationBuilder.DropColumn(
                "BotRespectsEarned",
                "Statistics");

            migrationBuilder.DropColumn(
                "BragsDone",
                "Statistics");

            migrationBuilder.DropColumn(
                "CapsulesEarned",
                "Statistics");

            migrationBuilder.DropColumn(
                "CapsulesOpened",
                "Statistics");

            migrationBuilder.DropColumn(
                "ChestsEarned",
                "Statistics");

            migrationBuilder.DropColumn(
                "ChestsOpened",
                "Statistics");

            migrationBuilder.DropColumn(
                "Date",
                "ScheduledEvents");

            migrationBuilder.DropColumn(
                "VoiceUptimeEarned",
                "Quests");

            migrationBuilder.DropColumn(
                "OpenedSphere",
                "QuestProgress");

            migrationBuilder.DropColumn(
                "VoiceUptimeEarned",
                "QuestProgress");

            migrationBuilder.RenameColumn(
                "TokensSpent",
                "Statistics",
                "TokensSpentTotal");

            migrationBuilder.RenameColumn(
                "TokensEarned",
                "Statistics",
                "TokensEarnedTotal");

            migrationBuilder.RenameColumn(
                "TicketsSpent",
                "Statistics",
                "SphereOpenedTotal");

            migrationBuilder.RenameColumn(
                "TicketsEarned",
                "Statistics",
                "SphereEarnedTotal");

            migrationBuilder.RenameColumn(
                "SpheresOpened",
                "Statistics",
                "PurchasedItemsTotal");

            migrationBuilder.RenameColumn(
                "SpheresEarned",
                "Statistics",
                "MessagesSentTotal");

            migrationBuilder.RenameColumn(
                "RewindsEarned",
                "Statistics",
                "EssenceEarnedTotal");

            migrationBuilder.RenameColumn(
                "RewindsActivated",
                "Statistics",
                "CoinsSpentTotal");

            migrationBuilder.RenameColumn(
                "PurchasedItems",
                "Statistics",
                "CoinsEarnedTotal");

            migrationBuilder.RenameColumn(
                "MessagesSent",
                "Statistics",
                "ChestsOpenedTotal");

            migrationBuilder.RenameColumn(
                "EssenceSpent",
                "Statistics",
                "ChestsEarnedTotal");

            migrationBuilder.RenameColumn(
                "EssenceEarned",
                "Statistics",
                "CapsuleOpenedTotal");

            migrationBuilder.RenameColumn(
                "DoubleExpsEarned",
                "Statistics",
                "CapsuleEarnedTotal");

            migrationBuilder.RenameColumn(
                "DoubleExpsActivated",
                "Statistics",
                "BragTotal");

            migrationBuilder.RenameColumn(
                "CoinsSpent",
                "Statistics",
                "AttacksReceived");

            migrationBuilder.RenameColumn(
                "CoinsEarned",
                "Statistics",
                "AttacksDone");

            migrationBuilder.AlterColumn<uint>(
                                "Id",
                                "ScheduledEvents",
                                nullable: false,
                                oldClrType: typeof(int))
                            .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                            .OldAnnotation("MySql:ValueGenerationStrategy",
                                           MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                "DayId",
                "ScheduledEvents",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                "Hour",
                "ScheduledEvents",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                "Minute",
                "ScheduledEvents",
                nullable: false,
                defaultValue: 0);

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
    }
}
