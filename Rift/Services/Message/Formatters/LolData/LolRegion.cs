using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.LolData
{
    public class LolRegion : FormatterBase
    {
        public LolRegion() : base("$lolRegion") {}

        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var lolData = await Database.GetUserLolDataAsync(data.UserId);
            
            return await ReplaceData(message, lolData.SummonerRegion);
        }
    }
}
