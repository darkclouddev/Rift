using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Gifts
{
    public class GiftTargetMention : TemplateBase
    {
        public GiftTargetMention() : base(nameof(GiftTargetMention)) {}

        public override Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, $"<@{data.Gift.TargetId.ToString()}>");
        }
    }
}
