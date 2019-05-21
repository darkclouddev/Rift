using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;

namespace Rift.Services.Message.Formatters.Moderation
{
    public class ModDuration : FormatterBase
    {
        public ModDuration() : base("$modDuration") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, data.Moderation.Duration.HasValue
                ? data.Moderation.Duration.Value.Humanize()
                : Template);
        }
    }
}
