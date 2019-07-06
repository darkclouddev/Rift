using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Giveaway
{
    public class TicketGiveawayParticipantsCount : TemplateBase
    {
        public TicketGiveawayParticipantsCount() : base(nameof(TicketGiveawayParticipantsCount)) {}

        public override Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, data.Giveaway.TicketGiveaway.ParticipantsCount.ToString());
        }
    }
}