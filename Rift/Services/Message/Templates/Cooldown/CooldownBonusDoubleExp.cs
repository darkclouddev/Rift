using System;
using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;

namespace Rift.Services.Message.Templates.Cooldown
{
    public class CooldownBonusDoubleExp : TemplateBase
    {
        public CooldownBonusDoubleExp() : base(nameof(CooldownBonusDoubleExp))
        {
        }

        const string NotActive = "не активирован.";

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var cd = await DB.Cooldowns.GetAsync(data.UserId);

            return await ReplaceDataAsync(message, cd.DoubleExpTimeSpan == TimeSpan.Zero
                                              ? NotActive
                                              : $"осталось {cd.DoubleExpTimeSpan.Humanize(culture: RiftBot.Culture)}");
        }
    }
}
