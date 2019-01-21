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
            
            sb.AppendLine($"Покупка: {cooldowns.StoreTimeSpan.Humanize(1, toWords: cooldowns.StoreTimeSpan == TimeSpan.Zero)}");
            sb.AppendLine($"Атака: {cooldowns.AttackTimeSpan.Humanize(1, toWords: cooldowns.AttackTimeSpan == TimeSpan.Zero)}");
            sb.AppendLine($"Атака по вам: {cooldowns.BeingAttackedTimeSpan.Humanize(1, toWords: cooldowns.BeingAttackedTimeSpan == TimeSpan.Zero)}");
            sb.AppendLine($"Дейлик: {cooldowns.DailyChestTimeSpan.Humanize(1, toWords: cooldowns.DailyChestTimeSpan == TimeSpan.Zero)}");
            sb.AppendLine($"Похвастаться: {cooldowns.BragTimeSpan.Humanize(1, toWords: cooldowns.BragTimeSpan == TimeSpan.Zero)}");
            sb.AppendLine($"Подарок: {cooldowns.GiftTimeSpan.Humanize(1, toWords: cooldowns.GiftTimeSpan == TimeSpan.Zero)}");
            sb.AppendLine($"Двойной опыт: {cooldowns.DoubleExpTimeSpan.Humanize(1, toWords: cooldowns.DoubleExpTimeSpan == TimeSpan.Zero)}");
            sb.AppendLine($"Уважение ботов: {cooldowns.BotRespectTimeSpan.Humanize(1, toWords: cooldowns.BotRespectTimeSpan == TimeSpan.Zero)}");
            sb.AppendLine($"Обновление ранга: {cooldowns.LolAccountUpdateTimeSpan.Humanize(1, toWords: cooldowns.LolAccountUpdateTimeSpan == TimeSpan.Zero)} назад");
            
            var eb = new EmbedBuilder()
                .WithAuthor("Ваши таймеры")
                .WithDescription(sb.ToString());

            return eb.Build();
        }
    }
}
