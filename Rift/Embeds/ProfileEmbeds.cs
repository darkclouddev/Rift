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
                             $"{Settings.Emote.Coin} {inventory.Coins.ToString()} "
                             + $"{Settings.Emote.Token} {inventory.Tokens.ToString()} "
                             + $"{Settings.Emote.Chest} {inventory.Chests.ToString()} "
                             + $"{Settings.Emote.Sphere} {inventory.Spheres.ToString()} "
                             + $"{Settings.Emote.Capsule} {inventory.Capsules.ToString()}")
                   .AddField("Бонусы",
                             $"{Settings.Emote.PowerupDoubleExperience} {inventory.PowerupsDoubleExperience.ToString()} "
                             + $"{Settings.Emote.BotRespect} {inventory.PowerupsBotRespect.ToString()}"
                             , true)
                   .AddField("Билеты",
                             $"{Settings.Emote.UsualTickets} {inventory.UsualTickets.ToString()} "
                             + $"{Settings.Emote.RareTickets} {inventory.RareTickets.ToString()}"
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
