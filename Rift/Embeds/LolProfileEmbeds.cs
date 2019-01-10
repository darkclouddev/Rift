using Rift.Configuration;

using Discord;
using Discord.WebSocket;

namespace Rift.Embeds
{
    class LolProfileEmbeds
    {
        public static Embed NoRankUpdated =
            new EmbedBuilder()
                .WithAuthor("League of Legends", Settings.Emote.LolUrl)
                .WithDescription($"Информация о вашем ранге успешно обновлена.")
                .Build();

        public static Embed DMRankUpdated =
            new EmbedBuilder()
                .WithAuthor("League of Legends", Settings.Emote.LolUrl)
                .WithDescription($"Вы получили {Settings.Emote.Chest} 4 за повышение ранга.")
                .Build();

        public static Embed ChatRankUpdated(SocketGuildUser sgUser, string newRank)
        {
            return new EmbedBuilder()
                   .WithAuthor("League of Legends", Settings.Emote.LolUrl)
                   .WithDescription($"Призыватель **{sgUser.Username}** недавно поднял ранг: {newRank.ToLower()}")
                   .Build();
        }
    }
}
