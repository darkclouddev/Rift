using System;
using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;
using Humanizer.Localisation;

namespace Rift.Services.Message.Templates.Statistic
{
    public class StatVoiceUptime : TemplateBase
    {
        public StatVoiceUptime() : base(nameof(StatVoiceUptime))
        {
        }

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return await ReplaceDataAsync(message,
                TimeSpan.FromMinutes(data.Statistics.VoiceUptimeMinutes)
                    .Humanize(
                        minUnit: TimeUnit.Minute,
                        maxUnit: TimeUnit.Hour,
                        culture: RiftBot.Culture));
        }
    }
}
