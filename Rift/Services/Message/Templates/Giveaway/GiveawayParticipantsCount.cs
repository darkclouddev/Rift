using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Giveaway
{
    public class GiveawayParticipantsCount : TemplateBase
    {
        public GiveawayParticipantsCount() : base(nameof(GiveawayParticipantsCount))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return ReplaceDataAsync(message, data.Giveaway.Log.Participants.Length.ToString());
        }
    }
}
