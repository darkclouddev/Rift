using Rift.Configuration;

using Discord;
using Discord.WebSocket;

namespace Rift.Embeds
{
    class ActivateEmbeds
    {
        public static readonly Embed NoSuchPowerupEmbed =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription($"У вас нет в инвентаре данного бонуса.")
                .Build();

        public static readonly Embed DoubleExpNotExpiredEmbed =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription($"У вас уже активирован {Settings.Emote.PowerupDoubleExperience} двойной опыт, отслеживайте в профиле.")
                .Build();

        public static readonly Embed BotRespectNotExpiredEmbed =
            new EmbedBuilder()
                .WithAuthor("Ошибка", Settings.Emote.ExMarkUrl)
                .WithColor(226, 87, 76)
                .WithDescription($"У вас уже активировано {Settings.Emote.BotRespect} уважение ботов, отслеживайте в профиле.")
                .Build();

        public static readonly Embed DoubleExpSuccessEmbed =
            new EmbedBuilder()
                .WithAuthor("Успешно", Settings.Emote.VerifyUrl)
                .WithColor(73, 197, 105)
                .WithDescription($"Вы активировали {Settings.Emote.PowerupDoubleExperience} двойной опыт на 12 часов.")
                .Build();

        public static readonly Embed BotRespectSuccessEmbed =
            new EmbedBuilder()
                .WithAuthor("Успешно", Settings.Emote.VerifyUrl)
                .WithColor(73, 197, 105)
                .WithDescription($"Вы активировали {Settings.Emote.BotRespect} уважение ботов на 12 часов.")
                .Build();

        public static Embed BotRespectChatEmbed(SocketGuildUser sgUser)
        {
            return new EmbedBuilder()
                   .WithDescription($"Призыватель {sgUser.Mention} активировал {Settings.Emote.BotRespect} уважение ботов.")
                   .Build();
        }

        public static Embed DoubleExpChatEmbed(SocketGuildUser sgUser)
        {
            return new EmbedBuilder()
                   .WithDescription($"Призыватель {sgUser.Mention} активировал {Settings.Emote.PowerupDoubleExperience} двойной опыт.")
                   .Build();
        }
    }
}
