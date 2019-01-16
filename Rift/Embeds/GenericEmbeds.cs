using Rift.Configuration;

using Discord;

namespace Rift.Embeds
{
    class GenericEmbeds
    {
        public static readonly Embed Empty =
            new EmbedBuilder()
                .WithDescription("empty")
                .Build();

        public static readonly Embed UserNotFound =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(255, 0, 0)
                .WithDescription($"Пользователь не найден!")
                .Build();

        public static readonly Embed RoleNotFound =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(255, 0, 0)
                .WithDescription($"Роль не найдена!")
                .Build();

        public static readonly Embed Error =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription($"Обратитесь к хранителю ботов и опишите ваши действия, которые привели к возникновению данной ошибки.")
                .Build();
    }
}
