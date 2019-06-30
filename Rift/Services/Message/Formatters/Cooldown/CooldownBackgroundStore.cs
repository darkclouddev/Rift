using System;
using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;
using Humanizer.Localisation;

namespace Rift.Services.Message.Formatters.Cooldown
{
    public class CooldownBackgroundStore : FormatterBase
    {
        public CooldownBackgroundStore() : base("$cooldownBackgroundStore") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var cd = await DB.Cooldowns.GetAsync(data.UserId);

            return await ReplaceData(message, cd.BackgroundStoreTimeSpan.Humanize(minUnit: TimeUnit.Second));
        }
    }
}
