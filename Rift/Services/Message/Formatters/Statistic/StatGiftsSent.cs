using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Statistic
{
    public class StatGiftsSent : FormatterBase
    {
        public StatGiftsSent() : base("$statGiftsSent") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.GiftsSent.ToString());
        }
    }
}
