using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;

namespace Rift.Services.Message.Formatters.Economy
{
    public class BragCooldownRemaining : FormatterBase
    {
        public BragCooldownRemaining() : base("$bragCooldownRemaining") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var cd = await DB.Cooldowns.GetAsync(data.UserId);
            return await ReplaceData(message, cd.BotRespectTimeSpan.Humanize());
        }
    }
}
