using System.Threading.Tasks;

using Humanizer;
using Humanizer.Localisation;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Cooldown
{
    public class CooldownStreamer : TemplateBase
    {
        public CooldownStreamer() : base(nameof(CooldownStreamer))
        {
        }

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var cds = await DB.Cooldowns.GetAsync(data.UserId);

            return await ReplaceDataAsync(message, cds.StreamerVoteTimeSpan
                .Humanize(minUnit: TimeUnit.Second, maxUnit: TimeUnit.Day, culture: RiftBot.Culture));
        }
    }
}
