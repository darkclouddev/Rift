using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Quest
{
    public class QuestStageRewardString : TemplateBase
    {
        public QuestStageRewardString() : base(nameof(QuestStageRewardString))
        {
        }

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            if (!data.QuestStage.CompletionRewardId.HasValue)
            {
                TemplateError($"Stage reward is not set: Stage ID {data.QuestStage.Id.ToString()}");
                return message;
            }

            var rewardId = data.QuestStage.CompletionRewardId.Value;
            
            var reward = await DB.Rewards.GetAsync(rewardId);

            if (reward is null)
            {
                TemplateError($"Reward is null: ID {rewardId.ToString()}");
                return message;
            }

            return await ReplaceDataAsync(message, reward.ToString());
        }
    }
}
