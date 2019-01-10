using System;
using System.IO;

using Newtonsoft.Json;

namespace Rift.Configuration
{
    public static class Settings
    {
        const string SettingsFolderName = "config";

        static readonly string SettingsFolderPath = Path.Combine(RiftBot.AppPath, SettingsFolderName);

        static readonly string AppSettingsFilePath = Path.Combine(SettingsFolderPath, "app.json");
        static readonly string ChannelIdSettingsFilePath = Path.Combine(SettingsFolderPath, "channels.json");
        static readonly string ChatSettingsFilePath = Path.Combine(SettingsFolderPath, "chat.json");
        static readonly string DatabaseSettingsFilePath = Path.Combine(SettingsFolderPath, "database.json");
        static readonly string EconomySettingsFilePath = Path.Combine(SettingsFolderPath, "economy.json");
        static readonly string EmoteSettingsFilePath = Path.Combine(SettingsFolderPath, "emotes.json");
        static readonly string RoleIdSettingsFilePath = Path.Combine(SettingsFolderPath, "roles.json");
        static readonly string ThumbnailSettingsFilePath = Path.Combine(SettingsFolderPath, "thumbnails.json");

        public static App App;
        public static ChannelId ChannelId;
        public static Chat Chat;
        public static Database Database;
        public static Economy Economy;
        public static Emote Emote;
        public static RoleId RoleId;
        public static Thumbnail Thumbnail;

        static Settings()
        {
            ReloadAll();
        }

        static void EnsureConfigDirCreated()
        {
            if (!Directory.Exists(SettingsFolderPath))
            {
                var di = Directory.CreateDirectory(SettingsFolderPath);
                RiftBot.Log.Debug($"Creating config folder: {di.Name}");
            }
        }

        public static void ReloadAll()
        {
            EnsureConfigDirCreated();

            ReloadApp();
            ReloadChannels();
            ReloadChat();
            ReloadDatabase();
            ReloadEconomy();
            ReloadEmotes();
            ReloadRoles();
            ReloadThumbnails();
        }

        public static void ReloadApp()
        {
            App = LoadSettingsFromFile<App>(AppSettingsFilePath);
        }

        public static void ReloadChannels()
        {
            ChannelId = LoadSettingsFromFile<ChannelId>(ChannelIdSettingsFilePath);
        }

        public static void ReloadChat()
        {
            Chat = LoadSettingsFromFile<Chat>(ChatSettingsFilePath);
        }

        public static void ReloadDatabase()
        {
            Database = LoadSettingsFromFile<Database>(DatabaseSettingsFilePath);
        }

        public static void ReloadEconomy()
        {
            Economy = LoadSettingsFromFile<Economy>(EconomySettingsFilePath);
        }

        public static void ReloadEmotes()
        {
            Emote = LoadSettingsFromFile<Emote>(EmoteSettingsFilePath);
        }

        public static void ReloadRoles()
        {
            RoleId = LoadSettingsFromFile<RoleId>(RoleIdSettingsFilePath);
        }

        public static void ReloadThumbnails()
        {
            Thumbnail = LoadSettingsFromFile<Thumbnail>(ThumbnailSettingsFilePath);
        }

        static T LoadSettingsFromFile<T>(string path)
            where T : new()
        {
            if (!File.Exists(path))
                return new T();

            string jsonContent = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(jsonContent);
        }

        public static void Save(SettingsType type)
        {
            switch (type)
            {
                case SettingsType.App:
                    File.WriteAllText(AppSettingsFilePath, JsonConvert.SerializeObject(App, Formatting.Indented));
                    break;

                case SettingsType.ChannelId:
                    File.WriteAllText(ChannelIdSettingsFilePath, JsonConvert.SerializeObject(ChannelId, Formatting.Indented));
                    break;

                case SettingsType.Chat:
                    File.WriteAllText(ChatSettingsFilePath, JsonConvert.SerializeObject(Chat, Formatting.Indented));
                    break;

                case SettingsType.Economy:
                    File.WriteAllText(EconomySettingsFilePath, JsonConvert.SerializeObject(Economy, Formatting.Indented));
                    break;

                case SettingsType.Emote:
                    File.WriteAllText(EmoteSettingsFilePath, JsonConvert.SerializeObject(Emote, Formatting.Indented));
                    break;

                case SettingsType.RoleId:
                    File.WriteAllText(RoleIdSettingsFilePath, JsonConvert.SerializeObject(RoleId, Formatting.Indented));
                    break;

                case SettingsType.Thumbnail:
                    File.WriteAllText(ThumbnailSettingsFilePath, JsonConvert.SerializeObject(Thumbnail, Formatting.Indented));
                    break;

                case SettingsType.Database:
                default: throw new InvalidOperationException($"Tried to save Database settings which is not allowed to do at runtime!");
            }
        }
    }

    public enum SettingsType
    {
        App,
        ChannelId,
        Chat,
        Database,
        Economy,
        Emote,
        RoleId,
        Thumbnail,
    }
}
