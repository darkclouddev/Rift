using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;
using Humanizer.Localisation;

namespace Rift.Services.Message.Formatters.Statistic
{
    public class StatVoiceUptime : FormatterBase
    {
        public StatVoiceUptime() : base("$statVoiceUptime") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.VoiceUptime
                .Humanize(minUnit: TimeUnit.Minute, maxUnit: TimeUnit.Hour));
        }
    }
}
