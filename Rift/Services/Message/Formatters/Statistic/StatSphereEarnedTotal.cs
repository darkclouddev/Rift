using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Statistic
{
    public class StatSphereEarnedTotal : FormatterBase
    {
        public StatSphereEarnedTotal() : base("$statSphereEarnedTotal") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.SphereEarnedTotal.ToString());
        }
    }
}
