using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Guild.Channels
{
    public class ChannelSearch : FormatterBase
    {
        public ChannelSearch() : base("$channelSearch") {}

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, $"<#{Settings.ChannelId.Search}>");
        }
    }
}
