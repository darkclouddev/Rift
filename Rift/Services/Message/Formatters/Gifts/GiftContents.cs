using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Gifts
{
    public class GiftContents : FormatterBase
    {
        public GiftContents() : base("$giftContents") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, data.Gift.Reward.ToString());
        }
    }
}
