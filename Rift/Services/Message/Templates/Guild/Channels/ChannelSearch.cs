using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Guild.Channels
{
    public class ChannelSearch : TemplateBase
    {
        public ChannelSearch() : base(nameof(ChannelSearch))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return ReplaceDataAsync(message, $"<#{Settings.ChannelId.Search.ToString()}>");
        }
    }
}
