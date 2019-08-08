using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Economy
{
    public class BragMinions : TemplateBase
    {
        public BragMinions() : base(nameof(BragMinions))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var stats = data.Brag.Stats;
            var laneMinions = stats.TotalMinionsKilled;
            var neutralMinions = stats.NeutralMinionsKilled;
            var cs = (laneMinions + neutralMinions).ToString();

            return ReplaceDataAsync(message, cs);
        }
    }
}
