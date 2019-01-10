using System.Text;

using Rift.Configuration;
using Rift.Rewards;

using Discord;

namespace Rift.Embeds
{
    class ChestEmbeds
    {
        public static readonly Embed NoChestsEmbed =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription($"У вас нет сундуков в инвентаре.")
                .Build();

        public static Embed ChatEmbed(Reward reward, ulong userId)
        {
            var sb = new StringBuilder();

            if (reward.PowerupsDoubleExp > 0)
                sb.Append($" {Settings.Emote.PowerupDoubleExperience} {reward.PowerupsDoubleExp}");

            if (reward.PowerupsBotRespect > 0)
                sb.Append($" {Settings.Emote.BotRespect} {reward.PowerupsBotRespect}");

            if (reward.UsualTickets > 0)
                sb.Append($" {Settings.Emote.Ctickets} {reward.UsualTickets}");

            if (reward.RareTickets > 0)
                sb.Append($" {Settings.Emote.Gtickets} {reward.RareTickets}");

            if (sb.Length == 0)
                return null;

            return new EmbedBuilder()
                   .WithAuthor("Сундуки", Settings.Emote.ChestUrl)
                   .WithDescription($"Призывателю <@{userId}> выпало:{sb.ToString()}")
                   .Build();
        }

        public static Embed DMEmbed(Reward reward, uint amount)
        {
            var eb = new EmbedBuilder()
                     .WithTitle($"Открытие сундука")
                     .WithColor(243, 205, 87)
                     .WithThumbnailUrl(Settings.Thumbnail.Chest);
            if (amount == 1)
                eb.WithDescription($"Призыватель, вы только что открыли сундук.\n\nВ сундуке: {reward.RewardString}");
            else
                eb.WithDescription($"Призыватель, вы только что открыли {amount} сундук(ов).\n\nВ сундуках: {reward.RewardString}");
            return eb.Build();
        }
    }
}
