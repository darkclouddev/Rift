using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Statistic
{
    public class StatCapsuleEarned : FormatterBase
    {
        public StatCapsuleEarned() : base("$statCapsuleEarned") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return await ReplaceData(message, data.Statistics.CapsulesEarned.ToString());
        }
    }
}
