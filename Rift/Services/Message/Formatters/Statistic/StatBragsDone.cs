using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Statistic
{
    public class StatBragsDone : FormatterBase
    {
        public StatBragsDone() : base("$statBragsDone") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.BragsDone.ToString());
        }
    }
}
