using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Gifts
{
    public class GiftTargetMention : FormatterBase
    {
        public GiftTargetMention() : base("$giftTargetMention") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, $"<@{data.Gift.TargetId.ToString()}>");
        }
    }
}
