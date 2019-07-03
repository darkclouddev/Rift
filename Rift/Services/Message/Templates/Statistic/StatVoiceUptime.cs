using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;
using Humanizer.Localisation;

namespace Rift.Services.Message.Templates.Statistic
{
    public class StatVoiceUptime : TemplateBase
    {
        public StatVoiceUptime() : base(nameof(StatVoiceUptime)) {}

        public override async Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.VoiceUptime
                .Humanize(minUnit: TimeUnit.Minute, maxUnit: TimeUnit.Hour));
        }
    }
}
