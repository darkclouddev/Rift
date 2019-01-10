using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Rift.Configuration;

using Discord.Commands;

namespace Rift.Modules
{
    [Group("settings")]
    public class SettingsModule : RiftModuleBase
    {
        [Group("channels")]
        public class ChannelsModule : ModuleBase
        {
            [Command]
            public async Task Default(string property, ulong value)
            {
                var p = Settings.ChannelId.GetType().GetProperty(property, BindingFlags.IgnoreCase);

                if (p is null)
                {
                    await ReplyAsync($"Property not found: \"{property}\".\n\n**Object properties**: {String.Join(",", Settings.ChannelId.GetType().GetProperties().ToList())}");
                    return;
                }

                try
                {
                    p.SetValue(Settings.ChannelId, value);
                }
                catch
                {
                    await ReplyAsync($"Failed to assign property value, check console log for details.");
                    return;
                }

                await ReplyAsync($"\"{p.Name}\" has been set to \"{value}\"");
            }
        }

        [Group("reload")]
        public class ReloadModule : ModuleBase
        {
            [Command]
            public async Task Default()
            {
                Settings.ReloadAll();

                await ReplyAsync($"All settings reloaded successfully.");
            }

            [Command("app")]
            public async Task App()
            {
                Settings.ReloadApp();

                await ReplyAsync($"App settings reloaded successfully.");
            }

            [Command("channels")]
            public async Task Channels()
            {
                Settings.ReloadChannels();

                await ReplyAsync($"Channels reloaded successfully.");
            }

            [Command("chat")]
            public async Task Chat()
            {
                Settings.ReloadChat();

                await ReplyAsync($"Chat settings reloaded successfully.");
            }

            [Command("database")]
            public async Task Database()
            {
                Settings.ReloadChat();

                await ReplyAsync($"Database settings reloaded successfully.");
            }

            [Command("economy")]
            public async Task Economy()
            {
                Settings.ReloadEconomy();

                await ReplyAsync($"Economy settings reloaded successfully.");
            }

            [Command("emote")]
            public async Task Emote()
            {
                Settings.ReloadEmotes();

                await ReplyAsync($"Emotes reloaded successfully.");
            }

            [Command("roles")]
            public async Task Roles()
            {
                Settings.ReloadRoles();

                await ReplyAsync($"Roles reloaded successfully.");
            }
        }
    }
}
