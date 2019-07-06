using System.Linq;
using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Giveaway
{
    public class GiveawayWinnerList : TemplateBase
    {
        public GiveawayWinnerList() : base(nameof(GiveawayWinnerList)) {}

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var winners = string.Join('\n', data.Giveaway.Log.Winners.Select(x => $"<@{x.ToString()}>"));

            return ReplaceDataAsync(message, winners);
        }
    }
}