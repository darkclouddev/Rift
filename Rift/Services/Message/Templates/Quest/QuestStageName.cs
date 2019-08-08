using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Quest
{
    public class QuestStageName : TemplateBase
    {
        public QuestStageName() : base(nameof(QuestStageName))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return ReplaceDataAsync(message, data.QuestStage.Name);
        }
    }
}
