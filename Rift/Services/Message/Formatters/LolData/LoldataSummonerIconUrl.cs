using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.LolData
{
    public class LoldataSummonerIconUrl : FormatterBase
    {
        public LoldataSummonerIconUrl() : base("$loldataSummonerIconUrl") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var iconUrl = RiftBot.GetService<RiotService>().GetSummonerIconUrlById(data.LolStat.Summoner.ProfileIconId);
            return ReplaceData(message, iconUrl);
        }
    }
}
