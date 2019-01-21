using System;
using System.Text;
using Rift.Data.Models.Timestamps;

using Discord;

using Humanizer;
using Rift.Data.Models.Cooldowns;

namespace Rift.Embeds
{
    public class CooldownEmbeds
    {
        public static Embed DMEmbed(UserCooldowns cooldowns)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine($"Покупка: {cooldowns.LastStoreTimeSpan.Humanize(1, toWords: cooldowns.LastStoreTimeSpan == TimeSpan.Zero)}");
            sb.AppendLine($"Атака: {cooldowns.LastAttackTimeSpan.Humanize(1, toWords: cooldowns.LastAttackTimeSpan == TimeSpan.Zero)}");
            sb.AppendLine($"Атака по вам: {cooldowns.LastBeingAttackedTimeSpan.Humanize(1, toWords: cooldowns.LastBeingAttackedTimeSpan == TimeSpan.Zero)}");
            sb.AppendLine($"Дейлик: {cooldowns.LastDailyChestTimeSpan.Humanize(1, toWords: cooldowns.LastDailyChestTimeSpan == TimeSpan.Zero)}");
            sb.AppendLine($"Похвастаться: {cooldowns.LastBragTimeSpan.Humanize(1, toWords: cooldowns.LastBragTimeSpan == TimeSpan.Zero)}");
            sb.AppendLine($"Подаритть: {cooldowns.LastGiftTimeSpan.Humanize(1, toWords: cooldowns.LastGiftTimeSpan == TimeSpan.Zero)}");
            sb.AppendLine($"Двойной опыт: {cooldowns.DoubleExpTimeSpan.Humanize(1, toWords: cooldowns.DoubleExpTimeSpan == TimeSpan.Zero)}");
            sb.AppendLine($"Уважение ботов: {cooldowns.BotRespectTimeSpan.Humanize(1, toWords: cooldowns.BotRespectTimeSpan == TimeSpan.Zero)}");
            //sb.AppendLine($"Обновление ранга: {cooldowns.LastLolAccountUpdateTimeSpan.Humanize(1, toWords: cooldowns.LastLolAccountUpdateTimeSpan == TimeSpan.Zero)}");
            
            var eb = new EmbedBuilder()
                .WithAuthor("Ваши таймеры")
                .WithDescription(sb.ToString());

            return eb.Build();
        }
    }
}
