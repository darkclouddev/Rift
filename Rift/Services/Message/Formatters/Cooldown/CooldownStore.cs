using System;
using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;

namespace Rift.Services.Message.Formatters.Cooldown
{
    public class CooldownStore : FormatterBase
    {
        public CooldownStore() : base("$cooldownStore") {}

        const string Available = "доступно";

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var cd = await Database.GetUserCooldownsAsync(data.UserId);
            var dt = DateTime.UtcNow;

            return await ReplaceData(message, cd.StoreTimeSpan == TimeSpan.Zero
                ? Available
                : $"осталось {cd.RoleStoreTimeSpan.Humanize()}");
        }
    }
}
