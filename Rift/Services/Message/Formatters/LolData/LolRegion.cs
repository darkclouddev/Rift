using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.LolData
{
    public class LolRegion : FormatterBase
    {
        public LolRegion() : base("$lolRegion") {}

        public override async Task<RiftMessage> Format(RiftMessage message, ulong userId)
        {
            var lolData = await Database.GetUserLolDataAsync(userId);
            
            return await ReplaceData(message, lolData.SummonerRegion);
        }
    }
}
