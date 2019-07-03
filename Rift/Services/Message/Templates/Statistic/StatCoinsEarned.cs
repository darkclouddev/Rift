using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Statistic
{
    public class StatCoinsEarned : TemplateBase
    {
        public StatCoinsEarned() : base(nameof(StatCoinsEarned)) {}

        public override async Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.CoinsEarned.ToString());
        }
    }
}
