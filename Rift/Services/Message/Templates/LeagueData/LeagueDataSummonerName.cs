using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.LeagueData
{
    public class LeagueDataSummonerName : TemplateBase
    {
        public LeagueDataSummonerName() : base(nameof(LeagueDataSummonerName)) {}

        public override async Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            var lolData = await DB.LolData.GetAsync(data.UserId);
            
            return await ReplaceData(message, lolData.SummonerName);
        }
    }
}
