using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Statistic
{
    public class StatTokensEarned : TemplateBase
    {
        public StatTokensEarned() : base(nameof(StatTokensEarned)) {}

        public override async Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.TokensEarned.ToString());
        }
    }
}
