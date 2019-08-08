using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Quest
{
    public class QuestRewardString : TemplateBase
    {
        public QuestRewardString() : base(nameof(QuestRewardString))
        {
        }

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var reward = await DB.Rewards.GetAsync(data.Quest.RewardId);

            if (reward is null)
            {
                TemplateError($"Reward is null: ID {data.Quest.RewardId.ToString()}");
                return message;
            }

            return await ReplaceDataAsync(message, reward.ToString());
        }
    }
}
