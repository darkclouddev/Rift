using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Statistic
{
    public class StatPurchasedItems : FormatterBase
    {
        public StatPurchasedItems() : base("$statPurchasedItems") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.PurchasedItems.ToString());
        }
    }
}
