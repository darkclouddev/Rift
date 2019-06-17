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

            return ReplaceData(message, $"{kills} / {deaths} / {assists}");
        }
    }
}
