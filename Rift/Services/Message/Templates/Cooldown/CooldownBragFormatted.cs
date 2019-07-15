using System;
using System.Threading.Tasks;

using Humanizer;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Cooldown
{
    public class CooldownBragFormatted : TemplateBase
    {
        public CooldownBragFormatted() : base(nameof(CooldownBragFormatted))
        {
        }
        
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
