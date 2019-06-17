using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Economy
{
    public class BragMinions : FormatterBase
    {
        public BragMinions() : base("$bragMinions") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var stats = data.Brag.Stats;
            var laneMinions = stats.TotalMinionsKilled;
            var neutralMinions = stats.NeutralMinionsKilled;
            var cs = (laneMinions + neutralMinions).ToString();

            return ReplaceData(message, cs);
        }
    }
}
