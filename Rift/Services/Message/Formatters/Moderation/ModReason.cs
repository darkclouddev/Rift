using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Moderation
{
    public class ModReason : FormatterBase
    {
        public ModReason() : base("$modReason") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, data.Moderation.Reason);
        }
    }
}
