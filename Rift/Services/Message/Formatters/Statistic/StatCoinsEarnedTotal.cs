using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Statistic
{
    public class StatCoinsEarnedTotal : FormatterBase
    {
        public StatCoinsEarnedTotal() : base("$statCoinsEarnedTotal") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.CoinsEarnedTotal.ToString());
        }
    }
}
