using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Gifts
{
    public class GiftPrice : TemplateBase
    {
        public GiftPrice() : base(nameof(GiftPrice)) {}

        public override Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, Settings.Economy.GiftPrice.ToString());
        }
    }
}
