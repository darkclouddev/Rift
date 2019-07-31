using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Statistic
{
    public class StatCapsulesEarned : TemplateBase
    {
        public StatCapsulesEarned() : base(nameof(StatCapsulesEarned))
        {
        }

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return await ReplaceDataAsync(message, data.Statistics.CapsulesEarned.ToString());
        }
    }
}
