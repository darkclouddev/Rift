using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Giveaway
{
    public class GiveawayWinnersCount : TemplateBase
    {
        public GiveawayWinnersCount() : base(nameof(GiveawayWinnersCount)) {}

        public override Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, data.Giveaway.Stored.WinnersAmount.ToString());
        }
    }
}