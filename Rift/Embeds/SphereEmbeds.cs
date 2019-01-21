using Rift.Configuration;
using Rift.Rewards;

using Discord;

namespace Rift.Embeds
{
    class SphereEmbeds
    {
        public static readonly Embed NoSpheresEmbed =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription($"У вас нет сфер в инвентаре.")
                .Build();

        public static Embed ChatEmbed(ulong userId, Sphere sphere)
        {
            return new EmbedBuilder()
                   .WithColor(95, 198, 196)
                   .WithThumbnailUrl(Settings.Thumbnail.Sphere)
                   .WithAuthor("Сферы")
                   .WithDescription($"Призыватель <@{userId.ToString()}> только что открыл сферу.\n")
                   .AddField("Выпало из сферы", $"{sphere.RewardString}")
                   .Build();
        }

        public static Embed DMEmbed(Sphere sphere)
        {
            return new EmbedBuilder()
                   .WithColor(95, 198, 196)
                   .WithAuthor("Сферы")
                   .WithThumbnailUrl(Settings.Thumbnail.Sphere)
                   .WithDescription($"Вы открыли сферу и получили награду.\n")
                   .AddField("Выпало из сферы", $"{sphere.RewardString}")
                   .Build();
        }
    }
}
