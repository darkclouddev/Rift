using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Economy
{
    public class BragChampName : TemplateBase
    {
        public BragChampName() : base(nameof(BragChampName)) {}

        public override Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, data.Brag.ChampionName);
        }
    }
}
