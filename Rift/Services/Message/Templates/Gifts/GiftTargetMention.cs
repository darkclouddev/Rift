using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Gifts
{
    public class GiftTargetMention : TemplateBase
    {
        public GiftTargetMention() : base(nameof(GiftTargetMention)) {}

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return ReplaceDataAsync(message, $"<@{data.Gift.TargetId.ToString()}>");
        }
    }
}
