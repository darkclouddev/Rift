using System;

using Rift.Configuration;
using Rift.Services.Economy;

using IonicLib.Extensions;

using Discord;
using Discord.WebSocket;

namespace Rift.Embeds
{
    class StoreEmbeds
    {
        public static readonly Embed NoCoins =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription($"У вас не хватает монет для покупки.")
                .Build();

        public static readonly Embed NoTokens =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription($"У вас не хватает жетонов для покупки.")
                .Build();

        public static readonly Embed HasRole =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription($"У вас уже имеется выбранная роль.")
                .Build();

        public static Embed Cooldown(TimeSpan remainingTime)
        {
            return new EmbedBuilder()
                   .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                   .WithColor(226, 87, 76)
                   .WithDescription($"Необходимо подождать, попробуйте через {remainingTime.FormatTimeToString()}")
                   .Build();
        }

        public static Embed Chat(SocketGuildUser sgUser, StoreItem item)
        {
            return new EmbedBuilder()
                   .WithDescription($"Призыватель {sgUser.Mention} купил роль {item.Emote} {item.Name}.")
                   .Build();
        }

        public static Embed DMSuccess(StoreItem item, string balance)
        {
            return new EmbedBuilder()
                   .WithAuthor("Успешно", Settings.Emote.VerifyUrl)
                   .WithColor(73, 197, 105)
                   .WithDescription($"Вы приобрели в магазине: {item.Emote} {item.Name}.\n\n"
                                    + $"Монеты и жетоны после покупки: {balance}")
                   .Build();
        }
    }
}
