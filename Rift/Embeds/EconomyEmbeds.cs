using System;
using System.Linq;

using Rift.Data.Models.Users;

using Discord;

using Rift.Services;

namespace Rift.Embeds
{
    class EconomyEmbeds
    {
        public static Embed ActiveUsersEmbed(UserTopExp[] topTen)
        {
            return new EmbedBuilder()
                   .WithTitle($"Активные призыватели")
                   .WithColor(70, 178, 214)
                   .WithDescription($"Десять самых активных призывателей нашего сервера:\n")
                   .AddField("Призыватель", String.Join('\n', topTen.Select(x => EconomyService.GetUserNameById(x.UserId))), true)
                   .AddField("Уровень", string.Join('\n', topTen.Select(x => x.Level)), true)
                   .AddField("Очки опыта", string.Join('\n', topTen.Select(x => x.Experience)), true)
                   .Build();
        }

        public static Embed RichUsersEmbed(UserTopCoins[] topTen)
        {
            return new EmbedBuilder()
                   .WithTitle($"Богатые призыватели")
                   .WithColor(70, 178, 214)
                   .WithDescription($"Десять самых богатых призывателей нашего сервера:")
                   .AddField("Призыватель", string.Join('\n', topTen.Select(x => EconomyService.GetUserNameById(x.UserId))), true)
                   .AddField("Монеты", string.Join('\n', topTen.Select(x => x.Coins)), true)
                   .AddField("Жетоны", string.Join('\n', topTen.Select(x => x.Tokens)), true)
                   .Build();
        }
    }
}
