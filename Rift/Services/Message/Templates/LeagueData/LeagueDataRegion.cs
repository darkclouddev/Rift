using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.LeagueData
{
    public class LeagueDataRegion : TemplateBase
    {
        public LeagueDataRegion() : base(nameof(LeagueDataRegion))
        {
        }

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var leagueData = await DB.LeagueData.GetAsync(data.UserId);

            return await ReplaceDataAsync(message, leagueData.SummonerRegion.ToUpperInvariant());
        }
    }
}
