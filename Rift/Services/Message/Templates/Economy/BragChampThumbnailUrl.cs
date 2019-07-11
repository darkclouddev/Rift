using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Economy
{
    public class BragChampThumbnailUrl : TemplateBase
    {
        public BragChampThumbnailUrl() : base(nameof(BragChampThumbnailUrl))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return ReplaceDataAsync(message, data.Brag.ChampionPortraitUrl);
        }
    }
}
