using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Preconditions;
using Rift.Services;

using Discord.Commands;

namespace Rift.Modules
{
    [Group("settings")]
    public class SettingsModule : RiftModuleBase
    {
        [Group("reload")]
        [RequireAdmin]
        public class ReloadModule : ModuleBase
        {
            [Command]
            public async Task Default()
            {
                await Settings.ReloadAllAsync();

                await ReplyAsync("All settings reloaded successfully.");
            }

            [Command("app")]
            public async Task App()
            {
                await Settings.ReloadAppAsync();

                await ReplyAsync("App settings reloaded successfully.");
            }

            [Command("channels")]
            public async Task Channels()
            {
                await Settings.ReloadChannelsAsync();

                await ReplyAsync("Channels reloaded successfully.");
            }

            [Command("chat")]
            public async Task Chat()
            {
                await Settings.ReloadChatAsync();

                await ReplyAsync("Chat settings reloaded successfully.");
            }

            [Command("economy")]
            public async Task Economy()
            {
                await Settings.ReloadEconomyAsync();

                await ReplyAsync("Economy settings reloaded successfully.");
            }

            [Command("emotes")]
            public async Task Emotes()
            {
                await RiftBot.GetService<EmoteService>().ReloadEmotesAsync();
                await ReplyAsync("Reloaded emotes.");
            }

            [Command("roles")]
            public async Task Roles()
            {
                await Settings.ReloadRolesAsync();

                await ReplyAsync("Roles reloaded successfully.");
            }
        }

        [Group("save")]
        [RequireAdmin]
        public class SaveModule : ModuleBase
        {
            [Command("app")]
            public async Task App()
            {
                await Settings.SaveAppAsync();
                await ReplyAsync("App settings saved successfully.");
            }

            [Command("channels")]
            public async Task Channels()
            {
                await Settings.SaveChannelsAsync();
                await ReplyAsync("Channels saved successfully.");
            }

            [Command("chat")]
            public async Task Chat()
            {
                await Settings.SaveChatAsync();
                await ReplyAsync("Chat settings saved successfully.");
            }

            [Command("economy")]
            public async Task Economy()
            {
                await Settings.SaveEconomyAsync();
                await ReplyAsync("Economy settings saved successfully.");
            }

            [Command("roles")]
            public async Task Roles()
            {
                await Settings.SaveRolesAsync();
                await ReplyAsync("Roles saved successfully.");
            }

            [Command("thumbnails")]
            public async Task Thumbnails()
            {
                await Settings.SaveThumbnailsAsync();
                await ReplyAsync("Thumbnails saved successfully.");
            }
        }
    }
}
