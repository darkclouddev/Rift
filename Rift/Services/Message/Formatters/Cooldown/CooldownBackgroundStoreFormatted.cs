using System;
using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;
using Humanizer.Localisation;

namespace Rift.Services.Message.Formatters.Cooldown
{
    public class CooldownBackgroundStoreFormatted : FormatterBase
    {
        public CooldownBackgroundStoreFormatted() : base("$cooldownBackgroundStoreFormatted") {}

        const string Available = "доступно";

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var cd = await DB.Cooldowns.GetAsync(data.UserId);

            return await ReplaceData(message, cd.BackgroundStoreTimeSpan == TimeSpan.Zero
                ? Available
                : $"осталось {cd.BackgroundStoreTimeSpan.Humanize(minUnit: TimeUnit.Second)}");
        }
    }
}