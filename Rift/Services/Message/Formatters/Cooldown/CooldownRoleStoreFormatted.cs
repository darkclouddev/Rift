using System;
using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;
using Humanizer.Localisation;

namespace Rift.Services.Message.Formatters.Cooldown
{
    public class CooldownRoleStoreFormatted : FormatterBase
    {
        public CooldownRoleStoreFormatted() : base("$cooldownRoleStoreFormatted") {}

        const string Available = "доступно";

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var cd = await DB.Cooldowns.GetAsync(data.UserId);

            return await ReplaceData(message, cd.RoleStoreTimeSpan == TimeSpan.Zero
                ? Available
                : $"осталось {cd.RoleStoreTimeSpan.Humanize(minUnit: TimeUnit.Second)}");
        }
    }
}