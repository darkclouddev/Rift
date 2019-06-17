using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Statistic
{
    public class StatCapsuleEarnedTotal : FormatterBase
    {
        public StatCapsuleEarnedTotal() : base("$statCapsuleEarnedTotal") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.CapsuleEarnedTotal.ToString());
        }
    }
}
