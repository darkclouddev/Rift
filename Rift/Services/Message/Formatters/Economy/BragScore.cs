using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Economy
{
    public class BragScore : FormatterBase
    {
        public BragScore() : base("$bragScore") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var stats = data.Brag.Stats;
            var kills = stats.Kills.ToString();
            var deaths = stats.Deaths.ToString();
            var assists = stats.Assists.ToString();
            var laneMinions = stats.TotalMinionsKilled;
            var neutralMinions = stats.NeutralMinionsKilled;
            var cs = (laneMinions + neutralMinions).ToString();

            return ReplaceData(message, $"{kills} / {deaths} / {assists}, {cs} cs");
        }
    }
}
