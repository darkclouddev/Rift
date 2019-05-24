using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Economy
{
    public class BragChampThumbnailUrl : FormatterBase
    {
        public BragChampThumbnailUrl() : base("$bragChampThumbnailUrl") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, data.Brag.ChampionPortraitUrl);
        }
    }
}
