using System;
using System.Text;

using Rift.Data.Models.Cooldowns;

using Discord;

using Humanizer;

namespace Rift.Embeds
{
    public class CooldownEmbeds
    {
        public static Embed DMEmbed(UserCooldowns cooldowns)
        {
            var sb = new StringBuilder();

            sb.AppendLine(cooldowns.StoreTimeSpan != TimeSpan.Zero
                ? $"Покупка через {FormatTimeSpan(cooldowns.StoreTimeSpan)}."
                : "Покупка доступна.");

            sb.AppendLine(cooldowns.AttackTimeSpan != TimeSpan.Zero
                ? $"Атака через {FormatTimeSpan(cooldowns.AttackTimeSpan)}."
                : "Атака доступна.");

            sb.AppendLine(cooldowns.BeingAttackedTimeSpan != TimeSpan.Zero
                ? $"Вас смогут атаковать через {FormatTimeSpan(cooldowns.BeingAttackedTimeSpan)}."
                : "Вас могут атаковать.");

            sb.AppendLine(cooldowns.DailyChestTimeSpan != TimeSpan.Zero
                ? $"Ежедневная награда через {FormatTimeSpan(cooldowns.DailyChestTimeSpan)}."
                : "Ежедневная награда доступна.");

            sb.AppendLine(cooldowns.BragTimeSpan != TimeSpan.Zero
                ? $"Похвастаться через {FormatTimeSpan(cooldowns.BragTimeSpan)}."
                : "Похвастаться можно уже сейчас.");

            sb.AppendLine(cooldowns.GiftTimeSpan != TimeSpan.Zero
                ? $"Подарить через {FormatTimeSpan(cooldowns.GiftTimeSpan)}."
                : "Дарить можно уже сейчас.");

            sb.AppendLine(cooldowns.DoubleExpTimeSpan != TimeSpan.Zero
                ? $"Двойной опыт: {FormatTimeSpan(cooldowns.DoubleExpTimeSpan)}."
                : "Двойной опыт отсутствует.");

            sb.AppendLine(cooldowns.BotRespectTimeSpan != TimeSpan.Zero
                ? $"ДУважение ботов: {FormatTimeSpan(cooldowns.BotRespectTimeSpan)}."
                : "Уважение ботов отсутствует.");

            sb.AppendLine(cooldowns.BotRespectTimeSpan != TimeSpan.Zero
                ? $"Ранг обновлялся {FormatTimeSpan(cooldowns.BotRespectTimeSpan)} назад."
                : "Обновление ранга: только что.");

            var eb = new EmbedBuilder()
                .WithAuthor("Ваши таймеры")
                .WithDescription(sb.ToString());

            return eb.Build();
        }

        static string FormatTimeSpan(TimeSpan ts)
        {
            return ts.Humanize(precision: 1,
                               culture: RiftBot.Culture,
                               toWords: ts == TimeSpan.Zero);
        }
    }
}
