using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Statistic
{
    public class StatGiftsSent : TemplateBase
    {
        public StatGiftsSent() : base(nameof(StatGiftsSent)) {}

        public override async Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.GiftsSent.ToString());
        }
    }
}
