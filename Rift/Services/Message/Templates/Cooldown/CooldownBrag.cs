using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;
using Humanizer.Localisation;

namespace Rift.Services.Message.Templates.Cooldown
{
    public class CooldownBrag : TemplateBase
    {
        public CooldownBrag() : base(nameof(CooldownBrag))
        {
        }

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var cd = await DB.Cooldowns.GetAsync(data.UserId);

            return await ReplaceDataAsync(message, cd.BragTimeSpan.Humanize(minUnit: TimeUnit.Second));
        }
    }
}
