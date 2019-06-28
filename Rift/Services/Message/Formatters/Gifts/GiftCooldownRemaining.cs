using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;

namespace Rift.Services.Message.Formatters.Gifts
{
    public class GiftCooldownRemaining : FormatterBase
    {
        public GiftCooldownRemaining() : base("$giftCooldownRemaining") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var cooldowns = await DB.Cooldowns.GetAsync(data.UserId);

            return await ReplaceData(message, cooldowns.GiftTimeSpan.Humanize());
        }
    }
}
