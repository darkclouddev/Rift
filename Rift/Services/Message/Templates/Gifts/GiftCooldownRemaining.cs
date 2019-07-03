using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;

namespace Rift.Services.Message.Templates.Gifts
{
    public class GiftCooldownRemaining : TemplateBase
    {
        public GiftCooldownRemaining() : base(nameof(GiftCooldownRemaining)) {}

        public override async Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            var cooldowns = await DB.Cooldowns.GetAsync(data.UserId);

            return await ReplaceData(message, cooldowns.GiftTimeSpan.Humanize());
        }
    }
}
