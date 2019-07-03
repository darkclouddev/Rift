using System;
using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;

namespace Rift.Services.Message.Templates.Cooldown
{
    public class CooldownBonusBotRespect : TemplateBase
    {
        public CooldownBonusBotRespect() : base(nameof(CooldownBonusBotRespect)) {}

        const string NotActive = "не активирован.";

        public override async Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            var cd = await DB.Cooldowns.GetAsync(data.UserId);

            return await ReplaceData(message, cd.BotRespectTimeSpan == TimeSpan.Zero
                ? NotActive
                : $"осталось {cd.BotRespectTimeSpan.Humanize()}");
        }
    }
}
