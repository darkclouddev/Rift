using System;

using Rift.Configuration;

using IonicLib.Extensions;

using Discord;
using Discord.WebSocket;

using Rift.Data.Models.Users;

namespace Rift.Embeds
{
    class GiftEmbeds
    {
        public static readonly Embed SelfGiftEmbed =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription("Невозможно отправить подарок себе.")
                .Build();

        public static readonly Embed NoCoinsEmbed =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription("У вас не хватает монет для отправки подарка.")
                .Build();

        public static readonly Embed NoTokensEmbed =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription("У вас не хватает жетонов для отправки подарка.")
                .Build();

        public static readonly Embed IncorrectNumberEmbed =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription("Предмета с таким номером не существует.")
                .Build();

        public static Embed DMFromEmbed(SocketGuildUser toUser, UserInventory inventory)
        {
            return new EmbedBuilder()
                   .WithAuthor("Успешно", Settings.Emote.VerifyUrl)
                   .WithColor(73, 197, 105)
                   .WithDescription($"Призыватель {toUser.Username} успешно получил подарок.\n"
                                    + $"Монеты и жетоны после отправки подарка: "
                                    + $"{Settings.Emote.Coin} {inventory.Coins} {Settings.Emote.Token} {inventory.Tokens}")
                   .Build();
        }

        public static Embed DMToEmbed(SocketGuildUser fromUser, string giftString)
        {
            return new EmbedBuilder()
                   .WithDescription($"Призыватель {fromUser.Username} подарил вам {giftString}")
                   .Build();
        }

        public static Embed ChatEmbed(SocketGuildUser fromUser, SocketGuildUser toUser, string giftString)
        {
            return new EmbedBuilder()
                   .WithDescription($"Призывателю {toUser.Mention} подарили {giftString}")
                   .WithFooter($"Подарок был отправлен игроком {fromUser.Username}")
                   .Build();
        }

        public static Embed CooldownEmbed(ulong remainingSeconds)
        {
            return new EmbedBuilder()
                   .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                   .WithColor(226, 87, 76)
                   .WithDescription($"Необходимо подождать, попробуйте через {TimeSpan.FromSeconds(remainingSeconds).FormatTimeToString()}.")
                   .Build();
        }

        public static Embed RoleReward(string rewardString)
        {
            return new EmbedBuilder()
                   .WithAuthor("Еженедельные подарки")
                   .WithDescription($"Вы получили {rewardString}")
                   .Build();
        }
    }
}
