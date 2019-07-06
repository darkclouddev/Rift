using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Statistic
{
    public class StatBragsDone : TemplateBase
    {
        public StatBragsDone() : base(nameof(StatBragsDone)) {}

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return await ReplaceDataAsync(message, data.Statistics.BragsDone.ToString());
        }
    }
}
