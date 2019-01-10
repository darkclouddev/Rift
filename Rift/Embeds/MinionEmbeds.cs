using Rift.Services;
using Rift.Rewards;

using Discord;

namespace Rift.Embeds
{
    class MinionEmbeds
    {
        public static Embed MinionSpawned(MinionColor color, string minionThumbnail)
        {
            var eb = new EmbedBuilder()
                     .WithThumbnailUrl(minionThumbnail)
                     .WithDescription($"Убейте его первым и получите уникальную награду.\n\nНеобходимо написать в чат: `!убить миньона`");
            if (color == MinionColor.Blue)
                eb.WithAuthor("В чате миньон синей команды");
            else
                eb.WithAuthor("В чате миньон красной команды");
            return eb.Build();
        }

        public static Embed MinionKilled(MinionColor color, ulong userId, Reward reward)
        {
            return new EmbedBuilder()
                   .WithDescription($"Призыватель <@{userId}> убил миньона и забрал {reward.RewardString}")
                   .Build();
        }

        public static Embed MinionSuicided = new EmbedBuilder()
                                             .WithDescription($"Миньону было слишком одиноко, он умер от скуки.")
                                             .Build();
    }
}
