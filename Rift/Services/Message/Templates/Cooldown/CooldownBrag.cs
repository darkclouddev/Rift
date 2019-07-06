using System;
using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;

namespace Rift.Services.Message.Templates.Cooldown
{
    public class CooldownBrag : TemplateBase
    {
        public CooldownBrag() : base(nameof(CooldownBrag)) {}

        const string Available = "доступно";

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var cd = await DB.Cooldowns.GetAsync(data.UserId);

            return await ReplaceDataAsync(message, cd.BragTimeSpan == TimeSpan.Zero
                ? Available
                : $"осталось {cd.BragTimeSpan.Humanize()}");
        }
    }
}
