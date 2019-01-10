using Rift.Configuration;
using Rift.Data.Models.Statistics;

using Discord;

using MingweiSamuel.Camille.SummonerV4;

namespace Rift.Embeds
{
    class StatEmbeds
    {
        public static Embed UserStatisticsEmbed(UserStatistics statistics)
        {
            return new EmbedBuilder()
                   .WithAuthor("Ваша статистика на сервере")
                   .WithThumbnailUrl(Settings.Thumbnail.Statistic)
                   .WithDescription("Статистика и информация для достижений:\n\n"
                                    + $"Монеты (получено / потрачено)\n"
                                    + $"{statistics.CoinsEarnedTotal} / {statistics.CoinsSpentTotal}\n\n"
                                    + $"Жетоны (получено / потрачено)\n"
                                    + $"{statistics.TokensEarnedTotal} / {statistics.TokensSpentTotal}\n\n"
                                    + $"Сундуки (получено / открыто)\n"
                                    + $"{statistics.ChestsEarnedTotal} / {statistics.ChestsOpenedTotal}\n\n"
                                    + $"Магазин\n"
                                    + $"Вы купили **{statistics.PurchasedItemsTotal}** товаров.\n\n"
                                    + $"Подарки\n"
                                    + $"Вы отправили **{statistics.GiftsSent}** подарков.\n\n"
                                    + $"Атаки\n"
                                    + $"Вы атаковали **{statistics.AttacksDone}** раз.\n\n"
                                    + $"Хвастовство\n"
                                    + $"Вы похвастались **{statistics.BragTotal}** раз.\n\n"
                                    + $"Сообщения\n"
                                    + $"Вы написали **{statistics.MessagesSentTotal}** сообщений.")
                   .Build();
        }

        public static readonly Embed NoRankEmbed =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription($"Необходимо подтвердить игровой аккаунт.")
                .Build();

        public static Embed SuccessEmbed(string thumbnail, Summoner summoner, string soloqRanking)
        {
            return new EmbedBuilder()
                   .WithThumbnailUrl(thumbnail)
                   .WithAuthor("Ваш игровой аккаунт", Settings.Emote.LolUrl)
                   .WithDescription("Подробная информация о вашем аккаунте в игре:")
                   .AddField("Никнейм", summoner.Name, true)
                   .AddField("Уровень / Регион", $"{summoner.SummonerLevel}", true)
                   .AddField("Ранг (одиночная / парная)",
                             $"{soloqRanking}\n\n"
                             + $"Статистика игрового аккаунт обновляется автоматически.\n"
                             + $"Командой в чат `!похвастаться` хвастайтесь своими играми.")
                   .Build();
        }
    }
}
