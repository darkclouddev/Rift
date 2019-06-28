using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Statistic
{
    public class StatTokensEarned : FormatterBase
    {
        public StatTokensEarned() : base("$statTokensEarned") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.TokensEarned.ToString());
        }
    }
}
