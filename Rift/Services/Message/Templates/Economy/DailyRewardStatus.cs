using System;
using System.Threading.Tasks;

using Humanizer;
using Humanizer.Localisation;

using Rift.Configuration;
using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Economy
{
    public class DailyRewardStatus : TemplateBase
    {
        public DailyRewardStatus() : base(nameof(DailyRewardStatus))
        {
        }

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var cds = await DB.Cooldowns.GetAsync(data.UserId);

            var statusString = cds.DailyRewardTimeSpan == TimeSpan.Zero
                ? $"Написать первое сообщение за день в <#{Settings.ChannelId.Chat.ToString()}>."
                : $"Вы уже выполнили задание, подождите {cds.DailyRewardTimeSpan.Humanize(culture: RiftBot.Culture, minUnit: TimeUnit.Second)}.";
                

            return await ReplaceDataAsync(message, statusString);
        }
    }
}
