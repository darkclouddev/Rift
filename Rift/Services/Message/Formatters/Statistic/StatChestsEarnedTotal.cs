using System.Threading.Tasks;
using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Statistic
{
    public class StatChestsEarnedTotal : FormatterBase
    {
        public StatChestsEarnedTotal() : base("$statChestsEarnedTotal") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.ChestsEarnedTotal.ToString());
        }
    }
}
