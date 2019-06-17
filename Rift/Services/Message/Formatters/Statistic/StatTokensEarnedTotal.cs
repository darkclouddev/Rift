using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Statistic
{
    public class StatTokensEarnedTotal : FormatterBase
    {
        public StatTokensEarnedTotal() : base("$statTokensEarnedTotal") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.TokensEarnedTotal.ToString());
        }
    }
}
