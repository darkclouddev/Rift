using System;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Rift.Configuration
{
    public static class Settings
    {
        public static App App;
        public static ChannelId ChannelId;
        public static Chat Chat;
        public static Economy Economy;
        public static RoleId RoleId;
        public static Thumbnail Thumbnail;

        public static async Task ReloadAllAsync()
        {
            await ReloadAppAsync();
            await ReloadChannelsAsync();
            await ReloadChatAsync();
            await ReloadEconomyAsync();
            await ReloadRolesAsync();
            await ReloadThumbnailsAsync();
        }

        public static async Task SaveAllAsync()
        {
            await SaveAppAsync();
            await SaveChannelsAsync();
            await SaveChatAsync();
            await SaveEconomyAsync();
            await SaveRolesAsync();
            await SaveThumbnailsAsync();
        }

        public static async Task ReloadAppAsync()
        {
            App = await LoadSettingsAsync<App>(SettingsType.App);
        }

        public static async Task SaveAppAsync()
        {
            await SaveAsync(SettingsType.App, App);
        }

        public static async Task ReloadChannelsAsync()
        {
            ChannelId = await LoadSettingsAsync<ChannelId>(SettingsType.ChannelId);
        }

        public static async Task SaveChannelsAsync()
        {
            await SaveAsync(SettingsType.ChannelId, ChannelId);
        }

        public static async Task ReloadChatAsync()
        {
            Chat = await LoadSettingsAsync<Chat>(SettingsType.Chat);
        }

        public static async Task SaveChatAsync()
        {
            await SaveAsync(SettingsType.Chat, Chat);
        }

        public static async Task ReloadEconomyAsync()
        {
            Economy = await LoadSettingsAsync<Economy>(SettingsType.Economy);
        }

        public static async Task SaveEconomyAsync()
        {
            await SaveAsync(SettingsType.Economy, Economy);
        }

        public static async Task ReloadRolesAsync()
        {
            RoleId = await LoadSettingsAsync<RoleId>(SettingsType.RoleId);
        }

        public static async Task SaveRolesAsync()
        {
            await SaveAsync(SettingsType.RoleId, RoleId);
        }

        public static async Task ReloadThumbnailsAsync()
        {
            Thumbnail = await LoadSettingsAsync<Thumbnail>(SettingsType.Thumbnail);
        }

        public static async Task SaveThumbnailsAsync()
        {
            await SaveAsync(SettingsType.Thumbnail, Thumbnail);
        }

        static async Task<T> LoadSettingsAsync<T>(SettingsType type)
            where T : new()
        {
            var dbSettings = await DB.Settings.GetAsync((int) type);

            if (dbSettings?.Data is null)
            {
                var settings = new T();
                await DB.Settings.SetAsync((int) type, JsonConvert.SerializeObject(settings, Formatting.Indented));

                return settings;
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(dbSettings.Data);
            }
            catch (Exception ex)
            {
                RiftBot.Log.Fatal($"Failed to deserialize {type.ToString()} settings!");
                RiftBot.Log.Fatal(ex);
                return default;
            }
        }

        static async Task SaveAsync(SettingsType type, object data)
        {
            if (data is null)
            {
                RiftBot.Log.Warn($"Trying to write null {type.ToString()} settings, skipping.");
                return;
            }

            try
            {
                var json = JsonConvert.SerializeObject(data);
                await DB.Settings.SetAsync((int) type, json);
            }
            catch (Exception ex)
            {
                RiftBot.Log.Fatal($"Failed to write {type.ToString()} settings to database!");
                RiftBot.Log.Fatal(ex);
            }
        }
    }

    public enum SettingsType
    {
        App = 0,
        ChannelId = 1,
        Chat = 2,
        Economy = 3,
        RoleId = 4,
        Thumbnail = 5,
    }
}
