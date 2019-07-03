using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Moderation
{
    public class ModReason : TemplateBase
    {
        public ModReason() : base(nameof(ModReason)) {}

        public override Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, data.Moderation.Reason);
        }
    }
}
