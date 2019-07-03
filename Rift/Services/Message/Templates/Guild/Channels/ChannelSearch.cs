using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Guild.Channels
{
    public class ChannelSearch : TemplateBase
    {
        public ChannelSearch() : base(nameof(ChannelSearch)) {}

        public override Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, $"<#{Settings.ChannelId.Search.ToString()}>");
        }
    }
}
