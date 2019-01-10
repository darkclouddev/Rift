using Rift.Configuration;

using Discord;

namespace Rift.Embeds
{
    class RoleEmbeds
    {
        public static Embed Roles =
            new EmbedBuilder()
                .WithAuthor("Роли на сервере")
                .WithDescription("Редкие и платные роли выдаются навсегда.\n"
                                 + "Обычные роли можно покупать в магазине на 30 дней.\n"
                                 + "Уникальные роли падают из капсул и доступны на 60 дней.")
                .AddField("Обычные роли", $"{Settings.Emote.Chosen} Избранные\n"
                                          + $"{Settings.Emote.DarkStar} Темная звезда\n"
                                          + $"{Settings.Emote.Arcade} Аркадные\n"
                                          + $"{Settings.Emote.Debonairs} Галантные\n"
                                          + $"{Settings.Emote.StarGuardians} Звездные защитники\n"
                                          + $"{Settings.Emote.BloodMoon} Кровавая луна\n"
                                          + $"{Settings.Emote.Hextech} Хекстековые\n"
                                          + $"{Settings.Emote.Party} Тусовые\n"
                                          + $"{Settings.Emote.Mythic} Мифические\n"
                                          + $"{Settings.Emote.Victorious} Победоносные")
                .AddField("Уникальные роли", $"{Settings.Emote.Roles} Эпические\n"
                                             + $"{Settings.Emote.Roles} Хасаги\n"
                                             + $"{Settings.Emote.Roles} Вардилочка\n"
                                             + $"{Settings.Emote.Roles} Реворкнутый\n"
                                             + $"{Settings.Emote.Roles} Метовый")
                .AddField("Редкие роли", $"{Settings.Emote.Roles} Довольные поро\n"
                                         + $"{Settings.Emote.Roles} Храбрые поро\n"
                                         + $"{Settings.Emote.Roles} Охотники")
                .AddField("Платные роли",
                          $"{Settings.Emote.Legendary} Легендарные\n{Settings.Emote.Absolute} Абсолютные\n{Settings.Emote.Roles} Личные роли")
                .Build();

        public static Embed DonatedRoles =
            new EmbedBuilder()
                .WithAuthor("Возможности платных ролей")
                .WithDescription("\n")
                .AddField($"Возможности с легендарной ролью:",
                          $"• Еженедельные подарки: {Settings.Emote.Coin} 5000 {Settings.Emote.Ctickets} 4\n"
                          + $"• Возможность получать за хвастайся больше монет.\n"
                          + $"• Возможность ставить эмоции под сообщения в чате.\n"
                          + $"• Возможность получать каждый день дополнительную награду.")
                .AddField($"Возможности с абсолютной ролью:",
                          $"• Еженедельные подарки: {Settings.Emote.Coin} 10000 {Settings.Emote.Sphere} 1\n"
                          + $"• Возможность получать за хвастайся больше монет.\n"
                          + $"• Возможность ставить эмоции под сообщения в чате.\n"
                          + $"• Возможность получать каждый день дополнительную награду.\n"
                          + $"• Возможность использовать смайлы с других серверов (Discord Nitro)")
                .Build();
    }
}
