using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Quest
{
    public class QuestName : TemplateBase
    {
        public QuestName() : base(nameof(QuestName))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return ReplaceDataAsync(message, data.Quest.Name);
        }
    }
}
