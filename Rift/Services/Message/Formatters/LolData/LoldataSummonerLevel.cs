using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.LolData
{
    public class LoldataSummonerLevel : FormatterBase
    {
        public LoldataSummonerLevel() : base("$loldataSummonerLevel") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            if (data.LolStat.Summoner is null)
            {
                RiftBot.Log.Error($"Template \"{nameof(LoldataSummonerLevel)}\": No summoner data found.");
                return Task.FromResult(message);
            }

            return ReplaceData(message, data.LolStat.Summoner.SummonerLevel.ToString());
        }
    }
}
