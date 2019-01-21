﻿using Rift.Configuration;
using Rift.Rewards;

using Discord;

namespace Rift.Embeds
{
    class GiveawayEmbeds
    {
        public static readonly string FreeMessage = "Призыватели, @here, участвуйте в розыгрыше.";

        public static Embed ChatFree(Reward reward)
        {
            return new EmbedBuilder()
                   .WithColor(243, 205, 87)
                   .WithThumbnailUrl(Settings.Thumbnail.Giveaway)
                   .WithDescription($"Прямо сейчас проходит розыгрыш.\n"
                                    + $"Для участия необходимо нажать на иконку под этим сообщением.\n\n"
                                    + $"Разыгрывается: {reward.RewardString}")
                   .Build();
        }

        public static Embed ChatTicket(Reward reward, string ticketString, int userCnt)
        {
            return new EmbedBuilder()
                   .WithColor(243, 205, 87)
                   .WithThumbnailUrl(Settings.Thumbnail.Giveaway)
                   .WithDescription($"В розыгрыше участвует **{userCnt.ToString()}** призывателей\n"
                                    + $"Бот автоматически использует билет из вашего инвентаря.\n"
                                    + $"Для участия необходимо иметь {ticketString} в инвентаре.\n\n"
                                    + $"Разыгрывается: {reward.RewardString}.")
                   .Build();
        }

        public static Embed ChatWinner(ulong winner, Reward reward)
        {
            return new EmbedBuilder()
                   .WithColor(243, 205, 87)
                   .WithDescription($"Призыватель <@{winner.ToString()}> получил {reward.RewardString}.")
                   .Build();
        }

        public static Embed DMWinner(Reward reward)
        {
            return new EmbedBuilder()
                   .WithColor(243, 205, 87)
                   .WithDescription($"Вы победили в розыгрыше и получили {reward.RewardString}.")
                   .Build();
        }
    }
}
