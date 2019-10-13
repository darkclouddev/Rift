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

        public static async Task ReloadAllAsync()
        {
            await ReloadAppAsync();
            await ReloadChannelsAsync();
            await ReloadChatAsync();
            await ReloadEconomyAsync();
        }

        public static async Task SaveAllAsync()
        {
            await SaveAppAsync();
            await SaveChannelsAsync();
            await SaveChatAsync();
            await SaveEconomyAsync();
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
                RiftBot.Log.Fatal(ex, $"Failed to deserialize {type.ToString()} settings!");
                return default;
            }
        }

        static async Task SaveAsync(SettingsType type, object data)
        {
            if (data is null)
            {
                RiftBot.Log.Warning($"Trying to write null {type.ToString()} settings, skipping.");
                return;
            }

            try
            {
                var json = JsonConvert.SerializeObject(data);
                await DB.Settings.SetAsync((int) type, json);
            }
            catch (Exception ex)
            {
                RiftBot.Log.Fatal(ex, $"Failed to write {type.ToString()} settings to database!");
            }
        }
    }

    public enum SettingsType
    {
        App = 0,
        ChannelId = 1,
        Chat = 2,
        Economy = 3,
    }
}
