using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Statistic
{
    public class StatEssenceEarnedTotal : FormatterBase
    {
        public StatEssenceEarnedTotal() : base("$statEssenceEarnedTotal") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.EssenceEarnedTotal.ToString());
        }
    }
}
