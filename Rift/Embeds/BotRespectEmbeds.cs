using Rift.Rewards;

using Discord;

namespace Rift.Embeds
{
    class BotRespectEmbeds
    {
        public static Embed ChatEmbed =
            new EmbedBuilder()
                .WithDescription("Бот <@357607386732691457> подарил уникальные награды призывателям.")
                .Build();

        public static Embed DMEmbed(Reward reward)
        {
            return new EmbedBuilder()
                   .WithDescription($"Бот <@357607386732691457> подарил вам {reward.RewardString}.")
                   .Build();
        }
    }
}
