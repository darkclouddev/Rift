using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;
using Humanizer.Localisation;

namespace Rift.Services.Message.Formatters.Cooldown
{
    public class CooldownItemStore : FormatterBase
    {
        public CooldownItemStore() : base("$cooldownItemStore") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var cd = await DB.Cooldowns.GetAsync(data.UserId);

            return await ReplaceData(message, cd.ItemStoreTimeSpan.Humanize(minUnit: TimeUnit.Second));
        }
    }
}
