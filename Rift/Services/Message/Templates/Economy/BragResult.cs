using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Economy
{
    public class BragResult : TemplateBase
    {
        public BragResult() : base(nameof(BragResult)) {}

        public override Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, data.Brag.Stats.Win ? "победу" : "поражение");
        }
    }
}
