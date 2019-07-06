using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Gifts
{
    public class GiftPrice : TemplateBase
    {
        public GiftPrice() : base(nameof(GiftPrice)) {}

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return ReplaceDataAsync(message, Settings.Economy.GiftPrice.ToString());
        }
    }
}
