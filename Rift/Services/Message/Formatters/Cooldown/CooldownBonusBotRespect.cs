using System;
using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;

namespace Rift.Services.Message.Formatters.Cooldown
{
    public class CooldownBonusBotRespect : FormatterBase
    {
        public CooldownBonusBotRespect() : base("$cooldownBonusBotRespect") {}

        const string NotActive = "не активирован.";

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var cd = await Database.GetUserCooldownsAsync(data.UserId);

            return await ReplaceData(message, cd.BotRespectTimeSpan == TimeSpan.Zero
                ? NotActive
                : $"осталось {cd.BotRespectTimeSpan.Humanize()}");
        }
    }
}
