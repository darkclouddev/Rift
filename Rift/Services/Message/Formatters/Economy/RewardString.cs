using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Economy
{
    public class RewardString : FormatterBase
    {
        public RewardString() : base("$rewardString") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, data.Reward.Reward.ToString());
        }
    }
}
