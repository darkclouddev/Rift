using System;
using System.Linq;

using Rift.Configuration;
using Rift.Data.Models.Users;
using Rift.Services;

using Discord;

namespace Rift.Embeds
{
    class DonateEmbeds
    {
        public static Embed ChatDonateEmbed(ulong userId, decimal amount)
        {
            return new EmbedBuilder()
                   .WithAuthor($"Пожертвования", Settings.Emote.DonateUrl)
                   .WithDescription($"Призыватель <@{userId.ToString()}> отправил **{amount.ToString()}** рублей.")
                   .Build();
        }

        public static Embed ChatDonateLegendaryRoleRewardEmbed(ulong userId)
        {
            return new EmbedBuilder()
                   .WithDescription($"Призыватель <@{userId.ToString()}> получил роль {Settings.Emote.Legendary} легендарные\nза общую сумму пожертвований в размере **500** рублей.")
                   .Build();
        }

        public static Embed ChatDonateAbsoluteRoleRewardEmbed(ulong userId)
        {
            return new EmbedBuilder()
                   .WithDescription($"Призыватель <@{userId.ToString()}> получил роль {Settings.Emote.Absolute} абсолютные\nза общую сумму пожертвований в размере **3000** рублей")
                   .Build();
        }

        public static Embed StatisticEmbed(UserDonate[] topTen)
        {
            return new EmbedBuilder()
                   .WithAuthor($"Статистика донатеров сервера")
                   .WithDescription($"Топ донатеры League of Legends RU:")
                   .AddField("Призыватель",
                             String.Join('\n', topTen.Select(x => EconomyService.GetUserNameById(x.UserId))), true)
                   .AddField("Сумма", String.Join('\n', topTen.Select(x => x.Donate.ToString("0.00"))), true)
                   .Build();
        }
    }
}
