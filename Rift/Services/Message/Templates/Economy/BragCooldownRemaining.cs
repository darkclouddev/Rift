using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;

namespace Rift.Services.Message.Templates.Economy
{
    public class BragCooldownRemaining : TemplateBase
    {
        public BragCooldownRemaining() : base(nameof(BragCooldownRemaining))
        {
        }

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var cd = await DB.Cooldowns.GetAsync(data.UserId);
            return await ReplaceDataAsync(message, cd.BotRespectTimeSpan.Humanize());
        }
    }
}
