using System;
using System.Threading.Tasks;

using Humanizer;
using Humanizer.Localisation;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Quest
{
    public class QuestStageRemainingTime : TemplateBase
    {
        public QuestStageRemainingTime() : base(nameof(QuestStageRemainingTime))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var ts = data.QuestStage.EndDate - DateTime.UtcNow;

            return ReplaceDataAsync(message, ts.Humanize(culture: RiftBot.Culture, precision: 3, minUnit: TimeUnit.Second));
        }
    }
}
