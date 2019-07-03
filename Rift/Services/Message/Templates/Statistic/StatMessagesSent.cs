using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Statistic
{
    public class StatMessagesSent : TemplateBase
    {
        public StatMessagesSent() : base(nameof(StatMessagesSent)) {}

        public override async Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.MessagesSent.ToString());
        }
    }
}
