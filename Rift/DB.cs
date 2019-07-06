using System;

using Rift.Database;

namespace Rift
{
    public static class DB
    {
        public static readonly ActiveGiveaways ActiveGiveaways;
        public static readonly BackgroundInventory BackgroundInventory;
        public static readonly Cooldowns Cooldowns;
        public static readonly GiveawayLogs GiveawayLogs;
        public static readonly Giveaways Giveaways;
        public static readonly Inventory Inventory;
        public static readonly LolData LolData;
        public static readonly ModerationLogs ModerationLogs;
        public static readonly PendingUsers PendingUsers;
        public static readonly ProfileBackgrounds ProfileBackgrounds;
        public static readonly Quests Quests;
        public static readonly Rewards Rewards;
        public static readonly RoleInventory RoleInventory;
        public static readonly ScheduledEvents ScheduledEvents;
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
            ActiveGiveaways = new ActiveGiveaways();
            BackgroundInventory = new BackgroundInventory();
            Cooldowns = new Cooldowns();
            GiveawayLogs = new GiveawayLogs();
            Giveaways = new Giveaways();
            Inventory = new Inventory();
            LolData = new LolData();
            ModerationLogs = new ModerationLogs();
            PendingUsers = new PendingUsers();
            ProfileBackgrounds = new ProfileBackgrounds();
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
        public DatabaseException(string message) : base(message)
        {
        }
    }
}
