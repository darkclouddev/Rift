using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class SetQuestConditionsNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "WroteFirstMessage",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "VoiceTimeEarned",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(TimeSpan));

            migrationBuilder.AlterColumn<uint>(
                name: "UsualMonstersKilled",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                name: "RolesPurchased",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                name: "RareMonstersKilled",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<bool>(
                name: "OpenedSphere",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<uint>(
                name: "OpenedChests",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                name: "MessagesSent",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                name: "LevelReached",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                name: "GiveawaysParticipated",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                name: "GiftsSent",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                name: "GiftsReceivedFromUltraGay",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                name: "GiftsReceived",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<bool>(
                name: "GiftedStreamer",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "GiftedModerator",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "GiftedBotKeeper",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<uint>(
                name: "EpicMonstersKilled",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                name: "CoinsSpent",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                name: "CoinsReceived",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                name: "BragsDone",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                name: "BoughtChests",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<bool>(
                name: "ApprovedLolAccount",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<uint>(
                name: "ActivatedBotRespects",
                table: "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<bool>(
                name: "WroteFirstMessage",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "VoiceTimeEarned",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(TimeSpan));

            migrationBuilder.AlterColumn<uint>(
                name: "UsualMonstersKilled",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                name: "RolesPurchased",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                name: "RareMonstersKilled",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<bool>(
                name: "OpenedSphere",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<uint>(
                name: "OpenedChests",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                name: "MessagesSent",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                name: "LevelReached",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                name: "GiveawaysParticipated",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                name: "GiftsSent",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                name: "GiftsReceivedFromUltraGay",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                name: "GiftsReceived",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<bool>(
                name: "GiftedStreamer",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "GiftedModerator",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "GiftedBotKeeper",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<uint>(
                name: "EpicMonstersKilled",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                name: "CoinsSpent",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                name: "CoinsReceived",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                name: "BragsDone",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                name: "BoughtChests",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<bool>(
                name: "ApprovedLolAccount",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<uint>(
                name: "ActivatedBotRespects",
                table: "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "WroteFirstMessage",
                table: "Quests",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "VoiceTimeEarned",
                table: "Quests",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "UsualMonstersKilled",
                table: "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "RolesPurchased",
                table: "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "RareMonstersKilled",
                table: "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "OpenedSphere",
                table: "Quests",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "OpenedChests",
                table: "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "MessagesSent",
                table: "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "LevelReached",
                table: "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "GiveawaysParticipated",
                table: "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "GiftsSent",
                table: "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "GiftsReceivedFromUltraGay",
                table: "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "GiftsReceived",
                table: "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "GiftedStreamer",
                table: "Quests",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "GiftedModerator",
                table: "Quests",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "GiftedBotKeeper",
                table: "Quests",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "EpicMonstersKilled",
                table: "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "CoinsSpent",
                table: "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "CoinsReceived",
                table: "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "BragsDone",
                table: "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "BoughtChests",
                table: "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "ApprovedLolAccount",
                table: "Quests",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "ActivatedBotRespects",
                table: "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "WroteFirstMessage",
                table: "QuestProgress",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "VoiceTimeEarned",
                table: "QuestProgress",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "UsualMonstersKilled",
                table: "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "RolesPurchased",
                table: "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "RareMonstersKilled",
                table: "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "OpenedSphere",
                table: "QuestProgress",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "OpenedChests",
                table: "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "MessagesSent",
                table: "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "LevelReached",
                table: "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "GiveawaysParticipated",
                table: "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "GiftsSent",
                table: "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "GiftsReceivedFromUltraGay",
                table: "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "GiftsReceived",
                table: "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "GiftedStreamer",
                table: "QuestProgress",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "GiftedModerator",
                table: "QuestProgress",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "GiftedBotKeeper",
                table: "QuestProgress",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "EpicMonstersKilled",
                table: "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "CoinsSpent",
                table: "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "CoinsReceived",
                table: "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "BragsDone",
                table: "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "BoughtChests",
                table: "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "ApprovedLolAccount",
                table: "QuestProgress",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "ActivatedBotRespects",
                table: "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);
        }
    }
}
