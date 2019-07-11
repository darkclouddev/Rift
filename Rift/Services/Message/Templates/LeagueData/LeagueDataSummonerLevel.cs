using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.LeagueData
{
    public class LeagueDataSummonerLevel : TemplateBase
    {
        public LeagueDataSummonerLevel() : base(nameof(LeagueDataSummonerLevel))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            if (data.LolStat.Summoner is null)
            {
                TemplateError("No summoner data found.");
                return Task.FromResult(message);
            }

            return ReplaceDataAsync(message, data.LolStat.Summoner.SummonerLevel.ToString());
        }
    }
}
