using System;
using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;

namespace Rift.Services.Message.Formatters.Cooldown
{
    public class CooldownBonusDoubleExp : FormatterBase
    {
        public CooldownBonusDoubleExp() : base("$cooldownBonusDoubleExp") {}

        const string NotActive = "не активирован.";

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var cd = await DB.Cooldowns.GetAsync(data.UserId);

            return await ReplaceData(message, cd.DoubleExpTimeSpan == TimeSpan.Zero
                ? NotActive
                : $"осталось {cd.DoubleExpTimeSpan.Humanize()}");
        }
    }
}
