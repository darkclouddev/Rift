using System;

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rift.Migrations
{
    public partial class AddedEventTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduledEvents");

            migrationBuilder.CreateTable(
                name: "ActiveEvents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                              .Annotation("MySql:ValueGenerationStrategy",
                                          MySqlValueGenerationStrategy.IdentityColumn),
                    EventName = table.Column<string>(nullable: true),
                    StoredMessageId = table.Column<int>(nullable: false),
                    ChannelMessageId = table.Column<ulong>(nullable: false),
                    StartedBy = table.Column<ulong>(nullable: false),
                    StartedAt = table.Column<DateTime>(nullable: false),
                    DueTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_ActiveEvents", x => x.Id); });

            migrationBuilder.CreateTable(
                name: "EventLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                              .Annotation("MySql:ValueGenerationStrategy",
                                          MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    SpecialWinnerId = table.Column<ulong>(nullable: false),
                    ParticipantsAmount = table.Column<uint>(nullable: false),
                    Reward = table.Column<string>(nullable: true),
                    StartedBy = table.Column<ulong>(nullable: false),
                    StartedAt = table.Column<DateTime>(nullable: false),
                    Duration = table.Column<TimeSpan>(nullable: false),
                    FinishedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_EventLogs", x => x.Id); });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    StoredMessageId = table.Column<int>(nullable: false),
                    SharedRewardId = table.Column<int>(nullable: false),
                    SpecialRewardId = table.Column<int>(nullable: false),
                    Duration = table.Column<TimeSpan>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<ulong>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Events", x => x.Name); });

            migrationBuilder.CreateTable(
                name: "EventSchedule",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                              .Annotation("MySql:ValueGenerationStrategy",
                                          MySqlValueGenerationStrategy.IdentityColumn),
                    Month = table.Column<int>(nullable: false),
                    Day = table.Column<int>(nullable: false),
                    Hour = table.Column<int>(nullable: false),
                    Minute = table.Column<int>(nullable: false),
                    EventName = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_EventSchedule", x => x.Id); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActiveEvents");

            migrationBuilder.DropTable(
                name: "EventLogs");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "EventSchedule");

            migrationBuilder.CreateTable(
                name: "ScheduledEvents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                              .Annotation("MySql:ValueGenerationStrategy",
                                          MySqlValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    EventId = table.Column<int>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_ScheduledEvents", x => x.Id); });
        }
    }
}
