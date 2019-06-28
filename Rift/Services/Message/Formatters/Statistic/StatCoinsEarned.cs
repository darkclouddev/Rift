using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Statistic
{
    public class StatCoinsEarned : FormatterBase
    {
        public StatCoinsEarned() : base("$statCoinsEarned") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.CoinsEarned.ToString());
        }
    }
}
