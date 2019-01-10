using Rift.Configuration;
using Rift.Data.Models.Users;

using Discord;

namespace Rift.Embeds
{
    class ProfileEmbeds
    {
        public static Embed InventoryEmbed(UserInventory inventory)
        {
            return new EmbedBuilder()
                   .WithTitle("Ваш инвентарь")
                   .WithThumbnailUrl(Settings.Thumbnail.Inventory)
                   .WithDescription("Количество предметов, бонусов и билетов:")
                   .AddField("Предметы",
                             $"{Settings.Emote.Coin} {inventory.Coins} "
                             + $"{Settings.Emote.Token} {inventory.Tokens} "
                             + $"{Settings.Emote.Chest} {inventory.Chests} "
                             + $"{Settings.Emote.Sphere} {inventory.Spheres} "
                             + $"{Settings.Emote.Capsule} {inventory.Capsules}")
                   .AddField("Бонусы",
                             $"{Settings.Emote.PowerupDoubleExperience} {inventory.PowerupsDoubleExperience} "
                             + $"{Settings.Emote.BotRespect} {inventory.PowerupsBotRespect}"
                             , true)
                   .AddField("Билеты",
                             $"{Settings.Emote.Ctickets} {inventory.UsualTickets} "
                             + $"{Settings.Emote.Gtickets} {inventory.RareTickets}"
                             , true)

                   //.AddField("Информация о бонусах",
                   //	$"Бонусы {Settings.Emote.PowerupAttack} и {Settings.Emote.PowerupProtection} активируются автоматически.\n" +
                   //	$"Бонусы {Settings.Emote.PowerupDoubleExperience} и {Settings.Emote.BotRespect} активируются с помощью команд.")
                   //.AddField("Команды для активации бонусов",
                   //	$"Двойной опыт ({Settings.Emote.PowerupDoubleExperience}): `!активировать двойной опыт`\n" +
                   //	$"Уважение ботов ({Settings.Emote.BotRespect}): `!активировать уважение ботов`")
                   .WithFooter("Узнайте все команды бота: !команды.")
                   .Build();
        }

        public static Embed DailyMessageGiftEmbed(string rewards)
        {
            return new EmbedBuilder()
                   .WithAuthor("Ежедневное задание")
                   .WithDescription($"Вы получили {rewards} за первое сообщение дня.")
                   .Build();
        }
    }
}
