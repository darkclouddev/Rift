using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Giveaway
{
    public class TicketGiveawayParticipantsCount : TemplateBase
    {
        public TicketGiveawayParticipantsCount() : base(nameof(TicketGiveawayParticipantsCount))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return ReplaceDataAsync(message, data.Giveaway.TicketGiveaway.ParticipantsCount.ToString());
        }
    }
}
