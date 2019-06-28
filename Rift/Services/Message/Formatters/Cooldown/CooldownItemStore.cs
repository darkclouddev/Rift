using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;

namespace Rift.Services.Message.Formatters.Cooldown
{
    public class CooldownItemStore : FormatterBase
    {
        public CooldownItemStore() : base("$cooldownItemStore") {}

        const string Available = "доступно";

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var cd = await DB.Cooldowns.GetAsync(data.UserId);

            return await ReplaceData(message, cd.ItemStoreTimeSpan.Humanize());
        }
    }
}
