using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Statistic
{
    public class StatSpheresEarned : TemplateBase
    {
        public StatSpheresEarned() : base(nameof(StatSpheresEarned)) {}

        public override async Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.SpheresEarned.ToString());
        }
    }
}
