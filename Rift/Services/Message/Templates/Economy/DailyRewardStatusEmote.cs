using System;
using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Economy
{
    public class DailyRewardStatusEmote : TemplateBase
    {
        public DailyRewardStatusEmote() : base(nameof(DailyRewardStatusEmote))
        {
        }

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var cds = await DB.Cooldowns.GetAsync(data.UserId);

            var emoteName = cds.DailyRewardTimeSpan == TimeSpan.Zero ? "noach" : "ach";

            var emote = RiftBot.GetService<EmoteService>().GetEmoteString($"$emote{emoteName}");

            if (string.IsNullOrWhiteSpace(emote))
            {
                TemplateError($"Could not find emote \"{emoteName}\"");
                return message;
            }

            return await ReplaceDataAsync(message, emote);
        }
    }
}
