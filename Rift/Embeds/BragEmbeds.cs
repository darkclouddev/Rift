using System;

using Rift.Configuration;

using IonicLib.Extensions;

using Discord;

using MingweiSamuel.Camille.MatchV4;

namespace Rift.Embeds
{
    class BragEmbeds
    {
        public static readonly Embed NoDataEmbed =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription($"Необходимо подтвердить игровой аккаунт, чтобы хвастаться.")
                .Build();

        public static readonly Embed NoMatchesEmbed =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription($"Отсутствуют недавние матчи, самое время сыграть!")
                .Build();

        public static Embed SuccessEmbed(ulong userId, string champThumb, string champName, Participant player,
                                         string queue, uint coins)
        {
            var eb = new EmbedBuilder()
                .WithAuthor("League of Legends", Settings.Emote.LolUrl);

            if (!String.IsNullOrWhiteSpace(champThumb))
                eb.WithThumbnailUrl(champThumb);

            eb.WithDescription($"Призыватель <@{userId}> хвастается своей игрой.\n"
                               + $"Игроку выдаются монеты за {(player.Stats.Win ? "победу" : "поражение")} в данной игре: {Settings.Emote.Coin} {coins}")
              .AddField("Режим игры", $"{queue}", true)
              .AddField("Чемпион и счет",
                        $"{champName} ({player.Stats.Kills} / {player.Stats.Deaths} / {player.Stats.Assists})", true);

            return eb.Build();
        }

        public static Embed CooldownEmbed(ulong remainingSeconds)
        {
            return new EmbedBuilder()
                   .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                   .WithColor(226, 87, 76)
                   .WithDescription($"Вы недавно уже хвастались в чате, подождите {TimeSpan.FromSeconds(remainingSeconds).FormatTimeToString()}")
                   .Build();
        }
    }
}
