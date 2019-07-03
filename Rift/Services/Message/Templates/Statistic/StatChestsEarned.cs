using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Statistic
{
    public class StatChestsEarned : TemplateBase
    {
        public StatChestsEarned() : base(nameof(StatChestsEarned)) {}

        public override async Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.ChestsEarned.ToString());
        }
    }
}
