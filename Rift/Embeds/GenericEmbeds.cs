using Rift.Configuration;

using Discord;

namespace Rift.Embeds
{
    class GenericEmbeds
    {
        public static readonly Embed EmptyEmbed =
            new EmbedBuilder()
                .WithDescription("empty")
                .Build();

        public static readonly Embed ErrorEmbed =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription($"Обратитесь к хранителю ботов и опишите ваши действия, которые привели к возникновению данной ошибки.")
                .Build();
    }
}
