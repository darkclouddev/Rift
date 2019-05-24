using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Economy
{
    public class BragReward : FormatterBase
    {
        public BragReward() : base("$bragReward") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, data.Brag.Reward);
        }
    }
}
