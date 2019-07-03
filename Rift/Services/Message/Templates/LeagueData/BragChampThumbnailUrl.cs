using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.LeagueData
{
    public class BragChampThumbnailUrl : TemplateBase
    {
        public BragChampThumbnailUrl() : base(nameof(BragChampThumbnailUrl)) {}

        public override Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, data.Brag.ChampionPortraitUrl);
        }
    }
}
