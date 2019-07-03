using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Economy
{
    public class RewardString : TemplateBase
    {
        public RewardString() : base(nameof(RewardString)) {}

        public override Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, data.Reward.ToString());
        }
    }
}
