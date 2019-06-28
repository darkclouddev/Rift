using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Statistic
{
    public class StatMessagesSent : FormatterBase
    {
        public StatMessagesSent() : base("$statMessagesSent") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.MessagesSent.ToString());
        }
    }
}
