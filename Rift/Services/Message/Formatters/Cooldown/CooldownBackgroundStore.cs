using System;
using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;

namespace Rift.Services.Message.Formatters.Cooldown
{
    public class CooldownBackgroundStore : FormatterBase
    {
        public CooldownBackgroundStore() : base("$cooldownBackgroundStore") {}

        const string Available = "доступно";

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var cd = await Database.GetUserCooldownsAsync(data.UserId);

            return await ReplaceData(message, cd.BackgroundStoreTimeSpan == TimeSpan.Zero
                ? Available
                : $"осталось {cd.BackgroundStoreTimeSpan.Humanize()}");
        }
    }
}
