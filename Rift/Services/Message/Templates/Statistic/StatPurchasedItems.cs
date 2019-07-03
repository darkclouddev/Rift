using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Statistic
{
    public class StatPurchasedItems : TemplateBase
    {
        public StatPurchasedItems() : base(nameof(StatPurchasedItems)) {}

        public override async Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.PurchasedItems.ToString());
        }
    }
}
