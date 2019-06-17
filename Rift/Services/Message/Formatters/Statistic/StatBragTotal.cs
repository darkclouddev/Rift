using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Statistic
{
    public class StatBragTotal : FormatterBase
    {
        public StatBragTotal() : base("$statBragTotal") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.BragTotal.ToString());
        }
    }
}
