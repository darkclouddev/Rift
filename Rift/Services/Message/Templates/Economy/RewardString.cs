using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Economy
{
    public class RewardString : TemplateBase
    {
        public RewardString() : base(nameof(RewardString)) {}

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return ReplaceDataAsync(message, data.Reward.ToString());
        }
    }
}
