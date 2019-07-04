using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Giveaway
{
    public class GiveawayParticipantsCount : TemplateBase
    {
        public GiveawayParticipantsCount() : base(nameof(GiveawayParticipantsCount)) {}

        public override Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, data.Giveaway.Log.Participants.Length.ToString());
        }
    }
}