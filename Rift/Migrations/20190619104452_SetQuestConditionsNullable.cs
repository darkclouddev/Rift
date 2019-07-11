using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class SetQuestConditionsNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                "WroteFirstMessage",
                "Quests",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<TimeSpan>(
                "VoiceTimeEarned",
                "Quests",
                nullable: true,
                oldClrType: typeof(TimeSpan));

            migrationBuilder.AlterColumn<uint>(
                "UsualMonstersKilled",
                "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                "RolesPurchased",
                "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                "RareMonstersKilled",
                "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<bool>(
                "OpenedSphere",
                "Quests",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<uint>(
                "OpenedChests",
                "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                "MessagesSent",
                "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                "LevelReached",
                "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                "GiveawaysParticipated",
                "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                "GiftsSent",
                "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                "GiftsReceivedFromUltraGay",
                "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                "GiftsReceived",
                "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<bool>(
                "GiftedStreamer",
                "Quests",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                "GiftedModerator",
                "Quests",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                "GiftedBotKeeper",
                "Quests",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<uint>(
                "EpicMonstersKilled",
                "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                "CoinsSpent",
                "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                "CoinsReceived",
                "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                "BragsDone",
                "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                "BoughtChests",
                "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<bool>(
                "ApprovedLolAccount",
                "Quests",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<uint>(
                "ActivatedBotRespects",
                "Quests",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<bool>(
                "WroteFirstMessage",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<TimeSpan>(
                "VoiceTimeEarned",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(TimeSpan));

            migrationBuilder.AlterColumn<uint>(
                "UsualMonstersKilled",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                "RolesPurchased",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                "RareMonstersKilled",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<bool>(
                "OpenedSphere",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<uint>(
                "OpenedChests",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                "MessagesSent",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                "LevelReached",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                "GiveawaysParticipated",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                "GiftsSent",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                "GiftsReceivedFromUltraGay",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                "GiftsReceived",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<bool>(
                "GiftedStreamer",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                "GiftedModerator",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                "GiftedBotKeeper",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<uint>(
                "EpicMonstersKilled",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                "CoinsSpent",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                "CoinsReceived",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                "BragsDone",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<uint>(
                "BoughtChests",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<bool>(
                "ApprovedLolAccount",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<uint>(
                "ActivatedBotRespects",
                "QuestProgress",
                nullable: true,
                oldClrType: typeof(uint));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                "WroteFirstMessage",
                "Quests",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                "VoiceTimeEarned",
                "Quests",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "UsualMonstersKilled",
                "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "RolesPurchased",
                "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "RareMonstersKilled",
                "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                "OpenedSphere",
                "Quests",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "OpenedChests",
                "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "MessagesSent",
                "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "LevelReached",
                "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "GiveawaysParticipated",
                "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "GiftsSent",
                "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "GiftsReceivedFromUltraGay",
                "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "GiftsReceived",
                "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                "GiftedStreamer",
                "Quests",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                "GiftedModerator",
                "Quests",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                "GiftedBotKeeper",
                "Quests",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "EpicMonstersKilled",
                "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "CoinsSpent",
                "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "CoinsReceived",
                "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "BragsDone",
                "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "BoughtChests",
                "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                "ApprovedLolAccount",
                "Quests",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "ActivatedBotRespects",
                "Quests",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                "WroteFirstMessage",
                "QuestProgress",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                "VoiceTimeEarned",
                "QuestProgress",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "UsualMonstersKilled",
                "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "RolesPurchased",
                "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "RareMonstersKilled",
                "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                "OpenedSphere",
                "QuestProgress",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "OpenedChests",
                "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "MessagesSent",
                "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "LevelReached",
                "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "GiveawaysParticipated",
                "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "GiftsSent",
                "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "GiftsReceivedFromUltraGay",
                "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "GiftsReceived",
                "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                "GiftedStreamer",
                "QuestProgress",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                "GiftedModerator",
                "QuestProgress",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                "GiftedBotKeeper",
                "QuestProgress",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "EpicMonstersKilled",
                "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "CoinsSpent",
                "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "CoinsReceived",
                "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "BragsDone",
                "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "BoughtChests",
                "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                "ApprovedLolAccount",
                "QuestProgress",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                "ActivatedBotRespects",
                "QuestProgress",
                nullable: false,
                oldClrType: typeof(uint),
                oldNullable: true);
        }
    }
}
