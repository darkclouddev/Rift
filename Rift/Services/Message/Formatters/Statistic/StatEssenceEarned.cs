using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Statistic
{
    public class StatEssenceEarned : FormatterBase
    {
        public StatEssenceEarned() : base("$statEssenceEarned") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.EssenceEarned.ToString());
        }
    }
}
