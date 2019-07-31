using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Quest
{
    public class QuestStageTotalQuests : TemplateBase
    {
        public QuestStageTotalQuests() : base(nameof(QuestStageTotalQuests))
        {
        }

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var count = await DB.Quests.GetStageQuestCountAsync(data.QuestStage.Id);

            return await ReplaceDataAsync(message, count.ToString());
        }
    }
}
