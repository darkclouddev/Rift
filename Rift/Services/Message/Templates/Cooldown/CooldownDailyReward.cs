using System;
using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;

namespace Rift.Services.Message.Templates.Cooldown
{
    public class CooldownDailyReward : TemplateBase
    {
        public CooldownDailyReward() : base(nameof(CooldownDailyReward)) {}

        const string Available = "доступно";

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var cd = await DB.Cooldowns.GetAsync(data.UserId);

            return await ReplaceDataAsync(message, cd.DailyChestTimeSpan == TimeSpan.Zero
                ? Available
                : $"осталось {cd.DailyChestTimeSpan.Humanize()}");
        }
    }
}
