﻿using System;
using System.Threading.Tasks;

using Rift.Data.Models;

using Humanizer;
using Humanizer.Localisation;

namespace Rift.Services.Message.Formatters.Cooldown
{
    public class CooldownItemStoreFormatted : FormatterBase
    {
        public CooldownItemStoreFormatted() : base("$cooldownItemStoreFormatted") { }

        const string Available = "доступно";

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var cd = await DB.Cooldowns.GetAsync(data.UserId);

            return await ReplaceData(message, cd.ItemStoreTimeSpan == TimeSpan.Zero
                ? Available
                : $"осталось {cd.ItemStoreTimeSpan.Humanize(minUnit: TimeUnit.Second)}");
        }
    }
}