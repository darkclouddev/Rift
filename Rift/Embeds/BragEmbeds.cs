using System;

using Rift.Configuration;

using IonicLib.Extensions;

using Discord;

using MingweiSamuel.Camille.MatchV4;

namespace Rift.Embeds
{
    class BragEmbeds
    {
        public static readonly Embed NoData =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription($"Необходимо подтвердить игровой аккаунт, чтобы хвастаться.")
                .Build();

        public static readonly Embed NoMatches =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription($"Отсутствуют недавние матчи, самое время сыграть!")
                .Build();

        public static Embed Success(ulong userId, string champThumb, string champName, Participant player,
                                         string queue, uint coins)
        {
            var eb = new EmbedBuilder()
                .WithAuthor("League of Legends", Settings.Emote.LolUrl);

            if (!String.IsNullOrWhiteSpace(champThumb))
                eb.WithThumbnailUrl(champThumb);

            eb.WithDescription($"Призыватель <@{userId.ToString()}> хвастается своей игрой.\n"
                               + $"Игроку выдаются монеты за {(player.Stats.Win ? "победу" : "поражение")} в данной игре: {Settings.Emote.Coin} {coins.ToString()}")
              .AddField("Режим игры", $"{queue}", true)
              .AddField("Чемпион и счет",
                        $"{champName} ({player.Stats.Kills.ToString()} / {player.Stats.Deaths.ToString()} / {player.Stats.Assists.ToString()})", true);

            return eb.Build();
        }

        public static Embed Cooldown(TimeSpan remaining)
        {
            return new EmbedBuilder()
                   .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                   .WithColor(226, 87, 76)
                   .WithDescription($"Вы недавно уже хвастались в чате, подождите {remaining.FormatTimeToString()}")
                   .Build();
        }
    }
}
