using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Preconditions;

using IonicLib;
using IonicLib.Util;

using Discord;
using Discord.Commands;

namespace Rift.Modules
{
    public class ModsModule : RiftModuleBase
    {
        static Embed lvlHelpEmbed = null;

        [Command("lvlhelp")]
        [RequireModerator]
        public async Task LvlHelp()
        {
            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
                return;

            if (lvlHelpEmbed is null)
            {
                var lvlHelpEmbed = new EmbedBuilder()
                    .WithDescription($"{Settings.Emote.QuestionMark} Награды за уровни на сервере\n\n"
                                         + $"Проявляйте активность в общем чате и получайте монеты, сундуки и редкие жетоны. С поднятием уровня вам будут открываться дополнительные возможности с ботом и награды будут увеличиваться.\n\n"
                                         + $"Награды за 2 уровень:\n"
                                         + $"Все призыватели получают {Settings.Emote.Coin} 100 {Settings.Emote.Chest} 1")
                    .Build();
            }

            await chatChannel.SendEmbedAsync(lvlHelpEmbed);
        }
    }
}
