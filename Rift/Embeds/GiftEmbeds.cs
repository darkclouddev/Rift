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
        public static readonly Embed SelfGift =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription("Невозможно отправить подарок себе.")
                .Build();

        public static readonly Embed NoCoins =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription("У вас не хватает монет для отправки подарка.")
                .Build();

        public static readonly Embed NoTokens =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription("У вас не хватает жетонов для отправки подарка.")
                .Build();

        public static readonly Embed IncorrectNumber =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription("Предмета с таким номером не существует.")
                .Build();

        public static Embed DMFrom(SocketGuildUser toUser, UserInventory inventory)
        {
            return new EmbedBuilder()
                   .WithAuthor("Успешно", Settings.Emote.VerifyUrl)
                   .WithColor(73, 197, 105)
                   .WithDescription($"Призыватель {toUser.Username} успешно получил подарок.\n"
                                    + $"Монеты и жетоны после отправки подарка: "
                                    + $"{Settings.Emote.Coin} {inventory.Coins.ToString()} {Settings.Emote.Token} {inventory.Tokens.ToString()}")
                   .Build();
        }

        public static Embed DMTo(SocketGuildUser fromUser, string giftString)
        {
            return new EmbedBuilder()
                   .WithDescription($"Призыватель {fromUser.Username} подарил вам {giftString}")
                   .Build();
        }

        public static Embed Chat(SocketGuildUser fromUser, SocketGuildUser toUser, string giftString)
        {
            return new EmbedBuilder()
                   .WithDescription($"Призывателю {toUser.Mention} подарили {giftString}")
                   .WithFooter($"Подарок был отправлен игроком {fromUser.Username}")
                   .Build();
        }

        public static Embed Cooldown(TimeSpan remainingTime)
        {
            return new EmbedBuilder()
                   .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                   .WithColor(226, 87, 76)
                   .WithDescription($"Необходимо подождать, попробуйте через {remainingTime.FormatTimeToString()}.")
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
