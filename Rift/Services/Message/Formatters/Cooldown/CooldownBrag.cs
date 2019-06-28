using System;
using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;

namespace Rift.Services.Message.Formatters.Cooldown
{
    public class CooldownBrag : FormatterBase
    {
        public CooldownBrag() : base("$cooldownBrag") {}

        const string Available = "доступно";

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var cd = await DB.Cooldowns.GetAsync(data.UserId);

            return await ReplaceData(message, cd.BragTimeSpan == TimeSpan.Zero
                ? Available
                : $"осталось {cd.BragTimeSpan.Humanize()}");
        }
    }
}
