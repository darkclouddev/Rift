using System;
using System.IO;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Rift.Configuration
{
    public static class Settings
    {
        const string SettingsFolderName = "config";

        static readonly string settingsFolderPath;

        static readonly string appSettingsFilePath;
        static readonly string channelIdSettingsFilePath;
        static readonly string chatSettingsFilePath;
        static readonly string economySettingsFilePath;
        static readonly string emoteSettingsFilePath;
        static readonly string roleIdSettingsFilePath;
        static readonly string thumbnailSettingsFilePath;

        public static App App;
        public static ChannelId ChannelId;
        public static Chat Chat;
        public static Economy Economy;
        public static Emote Emote;
        public static RoleId RoleId;
        public static Thumbnail Thumbnail;

        static Settings()
        {
            settingsFolderPath = Path.Combine(RiftBot.GetContentRoot(), SettingsFolderName);

            appSettingsFilePath = Path.Combine(settingsFolderPath, "app.json");
            channelIdSettingsFilePath = Path.Combine(settingsFolderPath, "channels.json");
            chatSettingsFilePath = Path.Combine(settingsFolderPath, "chat.json");
            economySettingsFilePath = Path.Combine(settingsFolderPath, "economy.json");
            emoteSettingsFilePath = Path.Combine(settingsFolderPath, "emotes.json");
            roleIdSettingsFilePath = Path.Combine(settingsFolderPath, "roles.json");
            thumbnailSettingsFilePath = Path.Combine(settingsFolderPath, "thumbnails.json");

            ReloadAll();
        }

        static void EnsureConfigDirCreated()
        {
            if (!Directory.Exists(settingsFolderPath)) 
                Directory.CreateDirectory(settingsFolderPath);
        }

        public static void ReloadAll()
        {
            EnsureConfigDirCreated();

            ReloadApp();
            ReloadChannels();
            ReloadChat();
            ReloadEconomy();
            ReloadEmotes();
            ReloadRoles();
            ReloadThumbnails();
        }

        public static void ReloadApp()
        {
            App = LoadSettingsFromFile<App>(appSettingsFilePath);
        }

        public static void ReloadChannels()
        {
            ChannelId = LoadSettingsFromFile<ChannelId>(channelIdSettingsFilePath);
        }

        public static void ReloadChat()
        {
            Chat = LoadSettingsFromFile<Chat>(chatSettingsFilePath);
        }

        public static void ReloadEconomy()
        {
            Economy = LoadSettingsFromFile<Economy>(economySettingsFilePath);
        }

        public static void ReloadEmotes()
        {
            Emote = LoadSettingsFromFile<Emote>(emoteSettingsFilePath);
        }

        public static void ReloadRoles()
        {
            RoleId = LoadSettingsFromFile<RoleId>(roleIdSettingsFilePath);
        }

        public static void ReloadThumbnails()
        {
            Thumbnail = LoadSettingsFromFile<Thumbnail>(thumbnailSettingsFilePath);
        }

        static T LoadSettingsFromFile<T>(string path)
            where T : new()
        {
            if (!File.Exists(path))
                return new T();

            string jsonContent = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(jsonContent);
        }

        public static async Task Save(SettingsType type)
        {
            switch (type)
            {
                case SettingsType.App:
                    await File.WriteAllTextAsync(appSettingsFilePath, JsonConvert.SerializeObject(App, Formatting.Indented));
                    break;

                case SettingsType.ChannelId:
                    await File.WriteAllTextAsync(channelIdSettingsFilePath, JsonConvert.SerializeObject(ChannelId, Formatting.Indented));
                    break;

                case SettingsType.Chat:
                    await File.WriteAllTextAsync(chatSettingsFilePath, JsonConvert.SerializeObject(Chat, Formatting.Indented));
                    break;

                case SettingsType.Economy:
                    await File.WriteAllTextAsync(economySettingsFilePath, JsonConvert.SerializeObject(Economy, Formatting.Indented));
                    break;

                case SettingsType.Emote:
                    await File.WriteAllTextAsync(emoteSettingsFilePath, JsonConvert.SerializeObject(Emote, Formatting.Indented));
                    break;

                case SettingsType.RoleId:
                    await File.WriteAllTextAsync(roleIdSettingsFilePath, JsonConvert.SerializeObject(RoleId, Formatting.Indented));
                    break;

                case SettingsType.Thumbnail:
                    await File.WriteAllTextAsync(thumbnailSettingsFilePath, JsonConvert.SerializeObject(Thumbnail, Formatting.Indented));
                    break;
            }
        }
    }

    public enum SettingsType
    {
        App,
        ChannelId,
        Chat,
        Economy,
        Emote,
        RoleId,
        Thumbnail,
    }
}
