using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Giveaway
{
    public class GiveawayDescription : TemplateBase
    {
        public GiveawayDescription() : base(nameof(GiveawayDescription))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return ReplaceDataAsync(message, data.Giveaway.Stored.Description);
        }
    }
}
