using Rift.Configuration;

using Discord;

namespace Rift.Embeds
{
    class HelpEmbeds
    {
        public static readonly Embed Commands =
            new EmbedBuilder()
                .WithAuthor("Rift")
                .WithThumbnailUrl("https://cdn.discordapp.com/attachments/375009656441012225/508792978186436619/7aa38d3711498398.png")

                //.WithDescription($"\n")
                .AddField($"Профиль, достижения и статистика"
                          , $"{Settings.Emote.Profile} `!профиль`, {Settings.Emote.MarkedAchievement} `!достижения`.\n"
                            + $"Проверка статистики в системе бота: `!статистика`.")
                .AddField($"Инвентарь и другие команды:"
                          , $"{Settings.Emote.Inventory} Инвентарь: `!инвентарь`.\n"
                            + $"Команды для открытия сундуков:\n"
                            + $"{Settings.Emote.Chest} `!открыть сундук`, `!открыть все сундуки`.\n"
                            + $"Команды для открытия сфер и капсул:\n"
                            + $"{Settings.Emote.Sphere} `!открыть сферу` {Settings.Emote.Capsule} `!открыть капсулу`.\n"
                            + $"Команды для активации бонусов:\n"
                            + $"{Settings.Emote.PowerupDoubleExperience} `!активировать двойной опыт`.\n"
                            + $"{Settings.Emote.BotRespect} `!активировать уважение ботов`.\n"
                            + $"{Settings.Emote.Ward} `!активировать магический тотем`.")
                .AddField($"Подарки, магазин и атаки в чате:"
                          , $"{Settings.Emote.Gifts} `!подарки`, {Settings.Emote.Store} `!магазин`, {Settings.Emote.Attack} `!атаки`.\n\n"
                            + $"Полная информация о системе: <#{Settings.ChannelId.Information}>")
                .Build();

        public static readonly Embed Rules =
            new EmbedBuilder()
                .WithDescription("Правила голосового сервера")
                .Build();
    }
}
