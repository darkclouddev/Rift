using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Statistic
{
    public class StatEssenceEarned : TemplateBase
    {
        public StatEssenceEarned() : base(nameof(StatEssenceEarned)) {}

        public override async Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.EssenceEarned.ToString());
        }
    }
}
