using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.LeagueData
{
    public class LeagueRankChange : TemplateBase
    {
        public LeagueRankChange() : base(nameof(LeagueRankChange))
        {
        }

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return await ReplaceDataAsync(message, data.RankData.CurrentRank);
        }
    }
}
