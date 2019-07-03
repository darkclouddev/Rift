using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Bot
{
    public class BotName : TemplateBase
    {
        public BotName() : base(nameof(BotName)) {}

        public override Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, nameof(Rift));
        }
    }
}
