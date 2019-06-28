using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Statistic
{
    public class StatSpheresEarned : FormatterBase
    {
        public StatSpheresEarned() : base("$statSpheresEarned") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.SpheresEarned.ToString());
        }
    }
}
