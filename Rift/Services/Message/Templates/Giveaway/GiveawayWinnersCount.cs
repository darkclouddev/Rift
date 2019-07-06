using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Giveaway
{
    public class GiveawayWinnersCount : TemplateBase
    {
        public GiveawayWinnersCount() : base(nameof(GiveawayWinnersCount)) {}

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return ReplaceDataAsync(message, data.Giveaway.Stored.WinnersAmount.ToString());
        }
    }
}