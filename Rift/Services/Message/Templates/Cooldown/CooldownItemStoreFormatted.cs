using System;
using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;
using Humanizer.Localisation;

namespace Rift.Services.Message.Templates.Cooldown
{
    public class CooldownItemStoreFormatted : TemplateBase
    {
        public CooldownItemStoreFormatted() : base(nameof(CooldownItemStoreFormatted)) { }

        const string Available = "доступно";

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var cd = await DB.Cooldowns.GetAsync(data.UserId);

            return await ReplaceDataAsync(message, cd.ItemStoreTimeSpan == TimeSpan.Zero
                ? Available
                : $"осталось {cd.ItemStoreTimeSpan.Humanize(minUnit: TimeUnit.Second)}");
        }
    }
}
