using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.LolData
{
    public class LolSummonerName : FormatterBase
    {
        public LolSummonerName() : base("$lolSummonerName") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var lolData = await Database.GetUserLolDataAsync(data.UserId);
            
            return await ReplaceData(message, lolData.SummonerName);
        }
    }
}
