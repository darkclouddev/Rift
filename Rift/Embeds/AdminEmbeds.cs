using Discord;

namespace Rift.Embeds
{
    class AdminEmbeds
    {
        public static readonly Embed ShutDown =
            new EmbedBuilder()
                .WithDescription($"Основной бот сервера выключен.")
                .Build();
    }
}
