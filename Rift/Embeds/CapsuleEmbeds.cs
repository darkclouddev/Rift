using Rift.Configuration;
using Rift.Rewards;

using Discord;

namespace Rift.Embeds
{
    class CapsuleEmbeds
    {
        public static readonly Embed NoCapsulesEmbed =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription($"У вас нет капсул в инвентаре.")
                .Build();

        public static Embed ChatEmbed(ulong userId, Capsule capsule)
        {
            var eb = new EmbedBuilder()
                     .WithColor(222, 171, 54)
                     .WithThumbnailUrl(Settings.Thumbnail.Capsule)
                     .WithAuthor("Редкая капсула")
                     .WithDescription($"Призыватель <@{userId}> открыл редкую капсулу.\n")
                     .AddField("Выпало из капсулы", capsule.RewardString);

            if (capsule.RoleId != 0)
                eb.AddField("Уникальная роль", capsule.RoleString);

            return eb.Build();
        }

        public static Embed DMEmbed(Capsule capsule)
        {
            var eb = new EmbedBuilder()
                     .WithColor(222, 171, 54)
                     .WithThumbnailUrl(Settings.Thumbnail.Capsule)
                     .WithAuthor("Редкая капсула")
                     .WithDescription($"Вы открыли капсулу и получили уникальную награду.\n")
                     .AddField("Выпало из капсулы", capsule.RewardString);

            if (capsule.RoleId != 0)
                eb.AddField("Уникальная роль", capsule.RoleString);

            return eb.Build();
        }
    }
}
