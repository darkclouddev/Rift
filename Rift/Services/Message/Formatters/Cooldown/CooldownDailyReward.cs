using System;
using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;

namespace Rift.Services.Message.Formatters.Cooldown
{
    public class CooldownDailyReward : FormatterBase
    {
        public CooldownDailyReward() : base("$cooldownDailyReward") {}

        const string Available = "доступно";

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var cd = await DB.Cooldowns.GetAsync(data.UserId);

            return await ReplaceData(message, cd.DailyChestTimeSpan == TimeSpan.Zero
                ? Available
                : $"осталось {cd.DailyChestTimeSpan.Humanize()}");
        }
    }
}
