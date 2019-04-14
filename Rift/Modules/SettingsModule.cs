using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Preconditions;

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
                Settings.ReloadAll();

                await ReplyAsync("All settings reloaded successfully.");
            }

            [Command("app")]
            public async Task App()
            {
                Settings.ReloadApp();

                await ReplyAsync("App settings reloaded successfully.");
            }

            [Command("channels")]
            public async Task Channels()
            {
                Settings.ReloadChannels();

                await ReplyAsync("Channels reloaded successfully.");
            }

            [Command("chat")]
            public async Task Chat()
            {
                Settings.ReloadChat();

                await ReplyAsync("Chat settings reloaded successfully.");
            }

            [Command("economy")]
            public async Task Economy()
            {
                Settings.ReloadEconomy();

                await ReplyAsync("Economy settings reloaded successfully.");
            }

            [Command("emote")]
            public async Task Emote()
            {
                Settings.ReloadEmotes();

                await ReplyAsync("Emotes reloaded successfully.");
            }

            [Command("roles")]
            public async Task Roles()
            {
                Settings.ReloadRoles();

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
                await Settings.Save(SettingsType.App);
                await ReplyAsync("App settings saved successfully.");
            }

            [Command("channels")]
            public async Task Channels()
            {
                await Settings.Save(SettingsType.ChannelId);
                await ReplyAsync("Channels saved successfully.");
            }

            [Command("chat")]
            public async Task Chat()
            {
                await Settings.Save(SettingsType.Chat);
                await ReplyAsync("Chat settings saved successfully.");
            }

            [Command("economy")]
            public async Task Economy()
            {
                await Settings.Save(SettingsType.Economy);
                await ReplyAsync("Economy settings saved successfully.");
            }

            [Command("emote")]
            public async Task Emote()
            {
                await Settings.Save(SettingsType.Emote);
                await ReplyAsync("Emotes saved successfully.");
            }

            [Command("roles")]
            public async Task Roles()
            {
                await Settings.Save(SettingsType.RoleId);
                await ReplyAsync("Roles saved successfully.");
            }

            [Command("thumbnails")]
            public async Task Thumbnails()
            {
                await Settings.Save(SettingsType.Thumbnail);
                await ReplyAsync("Thumbnails saved successfully.");
            }
        }
    }
}
