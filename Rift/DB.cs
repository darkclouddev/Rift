using System;

using Rift.Database;

namespace Rift
{
    public static class DB
    {
        public static ActiveGiveaways ActiveGiveaways;
        public static Cooldowns Cooldowns;
        public static GiveawayLogs GiveawayLogs;
        public static Giveaways Giveaways;
        public static Inventory Inventory;
        public static LolData LolData;
        public static ModerationLogs ModerationLogs;
        public static PendingUsers PendingUsers;
        public static Quests Quests;
        public static Rewards Rewards;
        public static RoleInventory RoleInventory;
        public static ScheduledEvents ScheduledEvents;
        public static Settings Settings;
        public static Statistics Statistics;
        public static StoredMessages StoredMessages;
        public static Streamers Streamers;
        public static SystemTimers SystemTimers;
        public static TempRoles TempRoles;
        public static Toxicity Toxicity;
        public static Users Users;

        static DB()
        {
            ActiveGiveaways = new ActiveGiveaways();
            Cooldowns = new Cooldowns();
            GiveawayLogs = new GiveawayLogs();
            Giveaways = new Giveaways();
            Inventory = new Inventory();
            LolData = new LolData();
            ModerationLogs = new ModerationLogs();
            PendingUsers = new PendingUsers();
            Quests = new Quests();
            Rewards = new Rewards();
            RoleInventory = new RoleInventory();
            ScheduledEvents = new ScheduledEvents();
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
        public new readonly string Message;

        public DatabaseException(string message) : base(message)
        {
            Message = message;
        }
    }
}
