using System.Text;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Preconditions;

using Discord.Commands;
using IonicLib;

namespace Rift.Modules
{
    [Group("filter")]
    public class FilterModule : RiftModuleBase
    {
        [Command]
        [RequireAdmin]
        [RequireContext(ContextType.Guild)]
        public async Task Default()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Фильтр чата {(Settings.Chat.CapsFilterEnabled ? "включён" : "выключен")}.");
            sb.AppendLine($"Фильтр вложений {(Settings.Chat.AttachmentFilterEnabled ? "включён" : "выключен")}.");
            sb.AppendLine($"Фильтр ссылок {(Settings.Chat.UrlFilterEnabled ? "включён" : "выключен")}.");

            await ReplyAsync(sb.ToString());
        }

        const string helpMessage = "**Доступные фильтры**\ncaps, attach, url";

        [Command("help")]
        [RequireAdmin]
        [RequireContext(ContextType.Guild)]
        public async Task Help()
        {
            await ReplyAsync(helpMessage);
        }

        [Command("caps")]
        [RequireAdmin]
        [RequireContext(ContextType.Guild)]
        public async Task Chat()
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Comms, out var channel))
                return;

            Settings.Chat.CapsFilterEnabled = !Settings.Chat.CapsFilterEnabled;
            await Settings.SaveChatAsync();

            await channel.SendMessageAsync($"Фильтр чата {(Settings.Chat.CapsFilterEnabled ? "включён" : "выключен")}.");
        }

        [Command("attach")]
        [RequireAdmin]
        [RequireContext(ContextType.Guild)]
        public async Task Attach()
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Comms, out var channel))
                return;

            Settings.Chat.AttachmentFilterEnabled = !Settings.Chat.AttachmentFilterEnabled;
            await Settings.SaveChatAsync();

            await channel.SendMessageAsync($"Фильтр вложений {(Settings.Chat.AttachmentFilterEnabled ? "включён" : "выключен")}.");
        }

        [Command("url")]
        [RequireAdmin]
        [RequireContext(ContextType.Guild)]
        public async Task URL()
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Comms, out var channel))
                return;

            Settings.Chat.UrlFilterEnabled = !Settings.Chat.UrlFilterEnabled;
            await Settings.SaveChatAsync();

            await channel.SendMessageAsync($"Фильтр ссылок {(Settings.Chat.UrlFilterEnabled ? "включён" : "выключен")}.");
        }
    }
}
