using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Event
{
    public class EventParticipantsCount : TemplateBase
    {
        public EventParticipantsCount() : base(nameof(EventParticipantsCount))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var count = data.EventData.Log.ParticipantsAmount.ToString();

            return ReplaceDataAsync(message, count);
        }
    }
}
