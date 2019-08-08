using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Quest
{
    public class QuestCurrentNumber : TemplateBase
    {
        public QuestCurrentNumber() : base(nameof(QuestCurrentNumber))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return ReplaceDataAsync(message, (data.Quest.Order + 1).ToString());
        }
    }
}
