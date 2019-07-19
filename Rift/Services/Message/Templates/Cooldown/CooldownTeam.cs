using System.Threading.Tasks;

using Humanizer;
using Humanizer.Localisation;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Cooldown
{
    public class CooldownTeam : TemplateBase
    {
        public CooldownTeam() : base(nameof(CooldownTeam))
        {
        }

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var cds = await DB.Cooldowns.GetAsync(data.UserId);

            return await ReplaceDataAsync(message, cds.LastTeamVoteTimeSpan.Humanize(minUnit: TimeUnit.Second));
        }
    }
}
