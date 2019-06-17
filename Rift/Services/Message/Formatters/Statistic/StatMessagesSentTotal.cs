using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Statistic
{
    public class StatMessagesSentTotal : FormatterBase
    {
        public StatMessagesSentTotal() : base("$statMessagesSentTotal") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.MessagesSentTotal.ToString());
        }
    }
}
