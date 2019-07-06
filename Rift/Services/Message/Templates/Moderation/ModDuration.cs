using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;

namespace Rift.Services.Message.Templates.Moderation
{
    public class ModDuration : TemplateBase
    {
        public ModDuration() : base(nameof(ModDuration)) {}

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return ReplaceDataAsync(message, data.Moderation.Duration.HasValue
                ? data.Moderation.Duration.Value.Humanize()
                : Template);
        }
    }
}
