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
            var iconUrl = RiftBot.GetService<RiotService>().GetSummonerIconUrlById(data.LolStat.Summoner.ProfileIconId);
            return ReplaceDataAsync(message, iconUrl);
        }
    }
}
