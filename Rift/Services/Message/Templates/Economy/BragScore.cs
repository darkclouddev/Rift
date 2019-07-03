using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Economy
{
    public class BragScore : TemplateBase
    {
        public BragScore() : base(nameof(BragScore)) {}

        public override Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            var stats = data.Brag.Stats;
            var kills = stats.Kills.ToString();
            var deaths = stats.Deaths.ToString();
            var assists = stats.Assists.ToString();

            return ReplaceData(message, $"{kills} / {deaths} / {assists}");
        }
    }
}
