using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Statistic
{
    public class StatCapsuleEarned : TemplateBase
    {
        public StatCapsuleEarned() : base(nameof(StatCapsuleEarned)) {}

        public override async Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.CapsulesEarned.ToString());
        }
    }
}
