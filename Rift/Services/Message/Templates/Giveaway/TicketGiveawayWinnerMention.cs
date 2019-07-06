using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Giveaway
{
    public class TicketGiveawayWinnerMention : TemplateBase
    {
        public TicketGiveawayWinnerMention() : base(nameof(TicketGiveawayWinnerMention)) {}

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return ReplaceDataAsync(message, $"<@{data.Giveaway.TicketGiveaway.WinnerId.ToString()}>");
        }
    }
}