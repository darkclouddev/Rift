using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Quest
{
    public class QuestStageDescription : TemplateBase
    {
        public QuestStageDescription() : base(nameof(QuestStageDescription))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return ReplaceDataAsync(message, data.QuestStage.Description);
        }
    }
}
