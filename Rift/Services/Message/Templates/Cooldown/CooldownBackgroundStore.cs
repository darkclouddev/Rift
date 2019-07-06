﻿using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;
using Humanizer.Localisation;

namespace Rift.Services.Message.Templates.Cooldown
{
    public class CooldownBackgroundStore : TemplateBase
    {
        public CooldownBackgroundStore() : base(nameof(CooldownBackgroundStore)) {}

        public override async Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            var cd = await DB.Cooldowns.GetAsync(data.UserId);

            return await ReplaceData(message, cd.BackgroundStoreTimeSpan.Humanize(minUnit: TimeUnit.Second));
        }
    }
}