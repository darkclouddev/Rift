using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Economy
{
    public class BragChampName : FormatterBase
    {
        public BragChampName() : base("$bragChampName") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, data.Brag.ChampionName);
        }
    }
}
