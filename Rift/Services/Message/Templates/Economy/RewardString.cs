using System.Threading.Tasks;

using Rift.Data.Models;
using Rift.Services.Reward;

namespace Rift.Services.Message.Templates.Economy
{
    public class RewardString : TemplateBase
    {
        public RewardString() : base(nameof(RewardString))
        {
        }

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var rewardString = data.Reward switch
            {
                ItemReward itemReward => data.RewardService.Format(itemReward),
                RoleReward roleReward => await data.RewardService.FormatAsync(roleReward),
                BackgroundReward backgroundReward => await data.RewardService.FormatAsync(backgroundReward),
                _ => "Пусто :("
            };

            return await ReplaceDataAsync(message, rewardString);
        }
    }
}
