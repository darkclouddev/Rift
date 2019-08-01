using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.LeagueData
{
    public class LeagueDataSummonerName : TemplateBase
    {
        public LeagueDataSummonerName() : base(nameof(LeagueDataSummonerName))
        {
        }

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return await ReplaceDataAsync(message, data.LeagueStat.Summoner.Name);
        }
    }
}
