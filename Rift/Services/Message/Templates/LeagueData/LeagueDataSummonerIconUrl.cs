using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.LeagueData
{
    public class LeagueDataSummonerIconUrl : TemplateBase
    {
        public LeagueDataSummonerIconUrl() : base(nameof(LeagueDataSummonerIconUrl))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var iconUrl = data.RiotService.GetSummonerIconUrlById(data.LeagueStat.Summoner.ProfileIconId);
            return ReplaceDataAsync(message, iconUrl);
        }
    }
}
