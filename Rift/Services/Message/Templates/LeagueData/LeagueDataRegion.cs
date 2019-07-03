using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.LeagueData
{
    public class LeagueDataRegion : TemplateBase
    {
        public LeagueDataRegion() : base(nameof(LeagueDataRegion)) { }

        public override async Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            var lolData = await DB.LolData.GetAsync(data.UserId);

            return await ReplaceData(message, lolData.SummonerRegion.ToUpperInvariant());
        }
    }
}
