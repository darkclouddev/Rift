using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Gifts
{
    public class GiftPrice : FormatterBase
    {
        public GiftPrice() : base("$giftPrice") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, Settings.Economy.GiftPrice.ToString());
        }
    }
}
