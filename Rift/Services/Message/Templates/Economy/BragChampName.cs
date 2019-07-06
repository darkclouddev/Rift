using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Economy
{
    public class BragChampName : TemplateBase
    {
        public BragChampName() : base(nameof(BragChampName)) {}

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return ReplaceDataAsync(message, data.Brag.ChampionName);
        }
    }
}
