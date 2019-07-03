using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.LeagueData
{
    public class LeagueDataSummonerIconUrl : TemplateBase
    {
        public LeagueDataSummonerIconUrl() : base(nameof(LeagueDataSummonerIconUrl)) {}

        public override Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            var iconUrl = RiftBot.GetService<RiotService>().GetSummonerIconUrlById(data.LolStat.Summoner.ProfileIconId);
            return ReplaceData(message, iconUrl);
        }
    }
}
