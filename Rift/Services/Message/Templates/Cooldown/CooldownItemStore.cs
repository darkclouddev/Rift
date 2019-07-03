using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;
using Humanizer.Localisation;

namespace Rift.Services.Message.Templates.Cooldown
{
    public class CooldownItemStore : TemplateBase
    {
        public CooldownItemStore() : base(nameof(CooldownItemStore)) {}

        public override async Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            var cd = await DB.Cooldowns.GetAsync(data.UserId);

            return await ReplaceData(message, cd.ItemStoreTimeSpan.Humanize(minUnit: TimeUnit.Second));
        }
    }
}
