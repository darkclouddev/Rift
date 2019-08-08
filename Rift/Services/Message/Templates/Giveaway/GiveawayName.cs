using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Giveaway
{
    public class GiveawayName : TemplateBase
    {
        public GiveawayName() : base(nameof(GiveawayName))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return ReplaceDataAsync(message, data.Giveaway.Stored.Name);
        }
    }
}
