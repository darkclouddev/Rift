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
        public static readonly Embed NoCoinsEmbed =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription($"У вас не хватает монет для покупки.")
                .Build();

        public static readonly Embed NoTokensEmbed =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription($"У вас не хватает жетонов для покупки.")
                .Build();

        public static Embed CooldownEmbed(ulong remainingSeconds)
        {
            return new EmbedBuilder()
                   .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                   .WithColor(226, 87, 76)
                   .WithDescription($"Необходимо подождать, попробуйте через {TimeSpan.FromSeconds(remainingSeconds).FormatTimeToString()}")
                   .Build();
        }

        public static Embed ChatEmbed(SocketGuildUser sgUser, StoreItem item)
        {
            return new EmbedBuilder()
                   .WithDescription($"Призыватель {sgUser.Mention} купил роль {item.Emote} {item.Name}.")
                   .Build();
        }

        public static Embed DMSuccessEmbed(StoreItem item, string balance)
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
