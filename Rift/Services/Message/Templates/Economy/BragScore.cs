using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Economy
{
    public class BragScore : TemplateBase
    {
        public BragScore() : base(nameof(BragScore))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var stats = data.Brag.Stats;
            var kills = stats.Kills.ToString();
            var deaths = stats.Deaths.ToString();
            var assists = stats.Assists.ToString();

            return ReplaceDataAsync(message, $"{kills} / {deaths} / {assists}");
        }
    }
}
