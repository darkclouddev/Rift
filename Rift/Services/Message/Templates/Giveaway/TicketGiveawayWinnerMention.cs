using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Giveaway
{
    public class TicketGiveawayWinnerMention : TemplateBase
    {
        public TicketGiveawayWinnerMention() : base(nameof(TicketGiveawayWinnerMention)) {}

        public override Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, $"<@{data.Giveaway.TicketGiveaway.WinnerId.ToString()}>");
        }
    }
}