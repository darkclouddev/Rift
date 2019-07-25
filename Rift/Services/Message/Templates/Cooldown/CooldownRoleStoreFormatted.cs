using System;
using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;
using Humanizer.Localisation;

namespace Rift.Services.Message.Templates.Cooldown
{
    public class CooldownRoleStoreFormatted : TemplateBase
    {
        public CooldownRoleStoreFormatted() : base(nameof(CooldownRoleStoreFormatted))
        {
        }

        const string Available = "доступно";

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var cd = await DB.Cooldowns.GetAsync(data.UserId);

            return await ReplaceDataAsync(message, cd.RoleStoreTimeSpan == TimeSpan.Zero
                                              ? Available
                                              : $"осталось {cd.RoleStoreTimeSpan.Humanize(minUnit: TimeUnit.Second, culture: RiftBot.Culture)}");
        }
    }
}
