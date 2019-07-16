using System;

using Rift.Database;

namespace Rift
{
    public static class DB
    {
        public static readonly ActiveEvents ActiveEvents;
        public static readonly ActiveGiveaways ActiveGiveaways;
        public static readonly BackgroundInventory BackgroundInventory;
        public static readonly Cooldowns Cooldowns;
        public static readonly Database.Events Events;
        public static readonly EventLogs EventLogs;
        public static readonly EventSchedule EventSchedule;
        public static readonly GiveawayLogs GiveawayLogs;
        public static readonly Giveaways Giveaways;
        public static readonly Inventory Inventory;
        public static readonly LeagueData LeagueData;
        public static readonly ModerationLogs ModerationLogs;
        public static readonly PendingUsers PendingUsers;
        public static readonly ProfileBackgrounds ProfileBackgrounds;
        public static readonly Quests Quests;
        public static readonly Rewards Rewards;
        public static readonly RoleInventory RoleInventory;
        public static readonly Roles Roles;
        public static readonly Settings Settings;
        public static readonly Statistics Statistics;
        public static readonly StoredMessages StoredMessages;
        public static readonly Streamers Streamers;
        public static readonly SystemTimers SystemTimers;
        public static readonly TempRoles TempRoles;
        public static readonly Toxicity Toxicity;
        public static readonly Users Users;

        static DB()
        {
            ActiveEvents = new ActiveEvents();
            ActiveGiveaways = new ActiveGiveaways();
            BackgroundInventory = new BackgroundInventory();
            Cooldowns = new Cooldowns();
            Events = new Database.Events();
            EventLogs = new EventLogs();
            EventSchedule = new EventSchedule();
            GiveawayLogs = new GiveawayLogs();
            Giveaways = new Giveaways();
            Inventory = new Inventory();
            LeagueData = new LeagueData();
            ModerationLogs = new ModerationLogs();
            PendingUsers = new PendingUsers();
            ProfileBackgrounds = new ProfileBackgrounds();
            Quests = new Quests();
            Rewards = new Rewards();
            RoleInventory = new RoleInventory();
            Roles = new Roles();
            Settings = new Settings();
            Statistics = new Statistics();
            StoredMessages = new StoredMessages();
            Streamers = new Streamers();
            SystemTimers = new SystemTimers();
            TempRoles = new TempRoles();
            Toxicity = new Toxicity();
            Users = new Users();
        }
    }

    public class DatabaseException : Exception
    {
        public DatabaseException(string message) : base(message)
        {
        }
    }
}
