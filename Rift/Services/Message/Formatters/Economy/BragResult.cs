using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Economy
{
    public class BragResult : FormatterBase
    {
        public BragResult() : base("$bragResult") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, data.Brag.Stats.Win ? "победу" : "поражение");
        }
    }
}
