using Rift.Configuration;
using Rift.Data.Models.Statistics;

using Discord;

namespace Rift.Embeds
{
    class LevelUpEmbeds
    {
        public static Embed Chat(ulong userId, uint newLevel)
        {
            return new EmbedBuilder()
                   .WithColor(107, 188, 239)
                   .WithDescription($"Призыватель <@{userId}> поднял {Settings.Emote.LevelUp} {newLevel} уровень на сервере.")
                   .Build();
        }

        public static Embed DM(uint level, UserStatistics statistics, string rewards)
        {
            return new EmbedBuilder()
                   .WithAuthor("Уровень")
                   .WithColor(107, 188, 239)
                   .WithThumbnailUrl(Settings.Thumbnail.LevelUp)
                   .WithDescription($"Вы подняли **{level}** уровень на сервере.\n\n"
                                    + $"Hаграда за повышение уровня: {rewards}")
                   .Build();
        }
    }
}
