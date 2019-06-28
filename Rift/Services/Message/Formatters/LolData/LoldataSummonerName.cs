using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.LolData
{
    public class LoldataSummonerName : FormatterBase
    {
        public LoldataSummonerName() : base("$loldataSummonerName") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var lolData = await DB.LolData.GetAsync(data.UserId);
            
            return await ReplaceData(message, lolData.SummonerName);
        }
    }
}
