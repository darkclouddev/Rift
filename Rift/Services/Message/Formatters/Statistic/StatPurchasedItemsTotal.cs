using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Statistic
{
    public class StatPurchasedItemsTotal : FormatterBase
    {
        public StatPurchasedItemsTotal() : base("$statPurchasedItemsTotal") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.PurchasedItemsTotal.ToString());
        }
    }
}
